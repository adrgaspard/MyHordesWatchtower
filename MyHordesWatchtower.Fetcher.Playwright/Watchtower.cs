using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Fetcher.Playwright.Abstractions;
using MyHordesWatchtower.Fetcher.Playwright.Models;
using MyHordesWatchtower.Fetcher.Playwright.Tools;
using System.Collections.Immutable;

namespace MyHordesWatchtower.Fetcher.Playwright;

[TestClass]
public class Watchtower : ScriptRunner
{
    private static readonly IReadOnlyList<int> SkippedCitizensIds = new[] { 16326 }.ToImmutableList();

    public Watchtower()
    {
        TimeoutInMs = 0;
        Headless = false;
        Locale = "fr-FR";
        AddAuthenticationCookies = true;
    }

    [TestMethod]
    public async Task Script()
    {
        List<CitizenBuilder> citizens = [];

        // Go to myhordes.eu
        IPage page = await Context.NewPageAsync();
        _ = await page.GotoAsync("https://myhordes.eu");
        await page.WaitForRandomDuration(500, 1000);

        // Fetch every citizens
        for (int i = 2; i <= 41; i++)
        {
            // Initialization & go to the citizens list
            CitizenBuilder builder = new();
            await page.Locator(".row > div:nth-child(4) > div").First.ClickAsync(); // Go to the citizens section
            _ = await page.WaitForResponseAsync("https://myhordes.eu/jx/town/citizens");
            await page.WaitForLoadingFinishedAsync();

            // Fetch identity
            IElementHandle? identityElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(3) a.username");
            string idStr = identityElement is null ? "" : await identityElement.GetAttributeAsync("x-user-id") ?? "";
            int id = idStr.ConvertToId();
            if (SkippedCitizensIds.Contains(id))
            {
                continue;
            }
            string pseudo = identityElement is null ? "" : await identityElement.TextContentAsync() ?? "";
            _ = builder.WithId(id).WithPseudo(pseudo);

            // Fetch profession
            IElementHandle? professionElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(3) img:first-child");
            string professionStr = professionElement is null ? "" : await professionElement.GetAttributeAsync("alt") ?? "";
            _ = builder.WithProfession(professionStr.ConvertToProfession());

            // Fetch chaman
            IElementHandle? chamanElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(3) > div > div > span > img");
            _ = builder.WithChaman(chamanElement is not null);

            // Fetch location
            IElementHandle? coordsElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(5)");
            string coordsStr = coordsElement is null ? "" : await coordsElement.TextContentAsync() ?? "";
            _ = builder.WithLocation(coordsStr.ConvertToLocation());

            // Fetch chaman
            IElementHandle? bannedElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(3) > div > div > div.inline > img");
            _ = builder.WithBanned(bannedElement is not null);

            // Fetch home level
            IElementHandle? homeLevelElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(4).citizen-box > img");
            string homeLevelStr = homeLevelElement is null ? "" : await homeLevelElement.GetAttributeAsync("alt") ?? "";
            _ = builder.WithHomeLevel(homeLevelStr.ConvertToHomeLevel());

            // Fetch defense
            IElementHandle? defenseElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({i}) > div:nth-child(4).citizen-box > span.citizen-defense");
            string? defenseStr = defenseElement is null ? null : await defenseElement.TextContentAsync();
            _ = builder.WithDefense(defenseStr.ConvertToDefense());

            // Go to the citizen homepage and fetch gossips
            await page.Locator($".citizens-list > div:nth-child({i}) > .userCell").First.ClickAsync(); // Open the citizen homepage
            await page.WaitForLoadingFinishedAsync();

            // Fetch state gossips
            IReadOnlyList<IElementHandle> gossipsElements = await page.QuerySelectorAllAsync("div.citizen-gossips + ul.gossips > li");
            List<string> gossipsStrings = (gossipsElements ?? Enumerable.Empty<IElementHandle>())
                .Select(element => element.TextContentAsync().Result)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .Cast<string>()
                .ToList();
            _ = builder.WithGossips(gossipsStrings.ConvertToCitizenGossips());

            // Fetch stars
            IReadOnlyList<IElementHandle>? starsElements = await page.QuerySelectorAllAsync("div.clairvoyance > img[alt='*']");
            _ = builder.WithStars(starsElements is not null ? (uint)starsElements.Count : 0);

            // Fetch death status
            IElementHandle? deathStatusElement = await page.QuerySelectorAsync("div.cell:has(> a[x-ajax-href])");
            string? deathStatusStr = deathStatusElement is null ? null : await deathStatusElement.TextContentAsync();
            _ = builder.WithDeathStatus(deathStatusStr.ConvertToDeathStatus());

            // Fetch charges count
            IReadOnlyList<IElementHandle> chargesElements = await page.QuerySelectorAllAsync("h5 + ul.gossips > li");
            _ = builder.WithChargesCount(chargesElements is null ? 0 : (uint)chargesElements.Count);

            // Fetch construction registry
            IReadOnlyList<IElementHandle> constructionsElements = await page.QuerySelectorAllAsync("div.row > div.cell > b");
            ImmutableDictionary<HomeConstructionType, uint> constructionsTuples = (constructionsElements ?? Enumerable.Empty<IElementHandle>())
                .Select(element => element.TextContentAsync().Result?.Trim().ToLower())
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .Cast<string>()
                .Select(str => str.Split(' '))
                .Where(array => array.Length > 1 || !uint.TryParse(array[^1], out _))
                .Select(array => (
                    array.Length > 1 ? string.Join(' ', array[..^1]) : array[0],
                    uint.TryParse(array[^1], out uint level) ? level : 1))
                .Select(tuple => (tuple.Item1.ConvertToHomeConstructionType(), tuple.Item2))
                .ToImmutableDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
            _ = builder.WithConstructions(constructionsTuples);

            // Fetch storage
            IElementHandle? storageElement = await page.QuerySelectorAsync("ul.inventory.chest");
            if (storageElement is null)
            {
                _ = builder.WithStorage(CitizenStorage.Invisible);
            }
            else
            {
                IReadOnlyList<IElementHandle> itemsElement = await page.QuerySelectorAllAsync("ul.inventory.chest > li.item > span.item-icon > img[alt]");
                ImmutableArray<Item> items = (constructionsElements ?? Enumerable.Empty<IElementHandle>())
                    .Select(element => element.GetAttributeAsync("alt").Result)
                    .Where(str => !string.IsNullOrWhiteSpace(str))
                    .Cast<string>()
                    .Select(str => new Item(str))
                    .ToImmutableArray();
                Dictionary<Item, uint> storageContent = [];
                foreach (Item? item in items)
                {
                    if (storageContent.TryGetValue(item, out uint value))
                    {
                        storageContent[item] = ++value;
                    }
                    else
                    {
                        storageContent.Add(item, 1);
                    }
                }
                _ = builder.WithStorage(CitizenStorage.CreateVisible(storageContent));
            }

            // Fetch decoration
            IElementHandle? decorationElement = await page.QuerySelectorAsync("div:nth-child(3) > div:nth-child(5) div:has(img)");
            string? decorationStr = decorationElement is null ? null : await decorationElement.TextContentAsync();
            _ = builder.WithDecoration(decorationStr.ConvertToDecoration());

            // Add citizen record
            citizens.Add(builder);
        }
    }
}