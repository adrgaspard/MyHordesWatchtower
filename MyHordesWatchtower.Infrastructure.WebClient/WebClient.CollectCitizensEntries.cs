using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using MyHordesWatchtower.Application;
using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Infrastructure.WebClient.Models;
using MyHordesWatchtower.Infrastructure.WebClient.Tools;
using System.Collections.Immutable;

namespace MyHordesWatchtower.Infrastructure.WebClient
{
    public sealed partial class WebClient : IWebClient
    {
        private async Task<IReadOnlyList<CitizenEntry>> CollectCitizensEntries(IBrowserContext context)
        {
            await context.AddAuthenticationCookiesAsync(_configuration);
            ImmutableList<int> skippedCitizensHordesIds = (_configuration.GetValue<int[]>("WebClient:SkippedCitizensHordesIds") ?? []).ToImmutableList();
            List<CitizenEntryBuilder> citizens = [];

            // Go to myhordes.eu
            IPage page = await context.NewPageAsync();
            _ = await page.GotoAsync("https://myhordes.eu");
            await page.WaitForRandomDuration(500, 1000);

            // Fetch every citizens
            for (int index = 2; index <= 41; index++)
            {
                CitizenEntryBuilder builder = new();
                await page.Locator(".row > div:nth-child(4) > div").First.ClickAsync(); // Go to the citizens section
                _ = await page.WaitForResponseAsync("https://myhordes.eu/jx/town/citizens");
                await page.WaitForLoadingFinishedAsync();

                if (await FetchIdentity(page, builder, index, skippedCitizensHordesIds))
                {
                    continue;
                }
                await FetchProfession(page, builder, index);
                await FetchChaman(page, builder, index);
                await FetchLocation(page, builder, index);
                await FetchBanned(page, builder, index);
                await FetchHomeLevel(page, builder, index);
                await FetchDefense(page, builder, index);

                await page.Locator($".citizens-list > div:nth-child({index}) > .userCell").First.ClickAsync(); // Open the citizen homepage
                await page.WaitForLoadingFinishedAsync();

                await FetchGossips(page, builder);
                await FetchStars(page, builder);
                await FetchDeathStatus(page, builder);
                await FetchChargesCount(page, builder);
                await FetchConstructionsRegistry(page, builder);
                await FetchStorage(page, builder);
                await FetchDecoration(page, builder);

                citizens.Add(builder);
            }

            Batch batch = new(new(Guid.NewGuid()), DateTime.Now);
            return citizens.Select(builder => new CitizenEntry(new(Guid.NewGuid()), batch, builder.Build())).ToImmutableList();
        }

        private static async Task<bool> FetchIdentity(IPage page, CitizenEntryBuilder builder, int index, IReadOnlyList<int> skippedCitizensHordesIds)
        {
            IElementHandle? identityElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(3) a.username");
            string idStr = identityElement is null ? "" : await identityElement.GetAttributeAsync("x-user-id") ?? "";
            int id = idStr.ConvertToId();
            if (skippedCitizensHordesIds.Contains(id))
            {
                return true;
            }
            string pseudo = identityElement is null ? "" : await identityElement.TextContentAsync() ?? "";
            _ = builder.WithId(id).WithPseudo(pseudo);
            return false;
        }

        private static async Task FetchProfession(IPage page, CitizenEntryBuilder builder, int index)
        {
            IElementHandle? professionElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(3) img:first-child");
            string professionStr = professionElement is null ? "" : await professionElement.GetAttributeAsync("alt") ?? "";
            _ = builder.WithProfession(professionStr.ConvertToProfession());
        }

        private static async Task FetchChaman(IPage page, CitizenEntryBuilder builder, int index)
        {
            IElementHandle? chamanElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(3) > div > div > span > img");
            _ = builder.WithChaman(chamanElement is not null);
        }

        private static async Task FetchLocation(IPage page, CitizenEntryBuilder builder, int index)
        {
            IElementHandle? coordsElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(5)");
            string coordsStr = coordsElement is null ? "" : await coordsElement.TextContentAsync() ?? "";
            _ = builder.WithLocation(coordsStr.ConvertToLocation());
        }

        private static async Task FetchBanned(IPage page, CitizenEntryBuilder builder, int index)
        {
            IElementHandle? bannedElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(3) > div > div > div.inline > img");
            _ = builder.WithBanned(bannedElement is not null);
        }

        private static async Task FetchHomeLevel(IPage page, CitizenEntryBuilder builder, int index)
        {
            IElementHandle? homeLevelElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(4).citizen-box > img");
            string homeLevelStr = homeLevelElement is null ? "" : await homeLevelElement.GetAttributeAsync("alt") ?? "";
            _ = builder.WithHomeLevel(homeLevelStr.ConvertToHomeLevel());
        }

        private static async Task FetchDefense(IPage page, CitizenEntryBuilder builder, int index)
        {
            IElementHandle? defenseElement = await page.QuerySelectorAsync($".citizens-list > div:nth-child({index}) > div:nth-child(4).citizen-box > span.citizen-defense");
            string? defenseStr = defenseElement is null ? null : await defenseElement.TextContentAsync();
            _ = builder.WithDefense(defenseStr.ConvertToDefense());
        }

        private static async Task FetchGossips(IPage page, CitizenEntryBuilder builder)
        {
            IReadOnlyList<IElementHandle> gossipsElements = await page.QuerySelectorAllAsync("div.citizen-gossips + ul.gossips > li");
            List<string> gossipsStrings = (gossipsElements ?? Enumerable.Empty<IElementHandle>())
                .Select(element => element.TextContentAsync().Result)
                .Where(str => !string.IsNullOrWhiteSpace(str))
                .Cast<string>()
                .ToList();
            _ = builder.WithGossips(gossipsStrings.ConvertToCitizenGossips());
        }

        private static async Task FetchStars(IPage page, CitizenEntryBuilder builder)
        {
            IReadOnlyList<IElementHandle>? starsElements = await page.QuerySelectorAllAsync("div.clairvoyance > img[alt='*']");
            _ = builder.WithStars(starsElements is not null ? (uint)starsElements.Count : 0);
        }

        private static async Task FetchDeathStatus(IPage page, CitizenEntryBuilder builder)
        {
            IElementHandle? deathStatusElement = await page.QuerySelectorAsync("div.cell:has(> a[x-ajax-href])");
            string? deathStatusStr = deathStatusElement is null ? null : await deathStatusElement.TextContentAsync();
            _ = builder.WithDeathStatus(deathStatusStr.ConvertToDeathStatus());
        }

        private static async Task FetchChargesCount(IPage page, CitizenEntryBuilder builder)
        {
            IReadOnlyList<IElementHandle> chargesElements = await page.QuerySelectorAllAsync("h5 + ul.gossips > li");
            _ = builder.WithChargesCount(chargesElements is null ? 0 : (uint)chargesElements.Count);
        }

        private static async Task FetchConstructionsRegistry(IPage page, CitizenEntryBuilder builder)
        {
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
        }

        private static async Task FetchStorage(IPage page, CitizenEntryBuilder builder)
        {
            IElementHandle? storageElement = await page.QuerySelectorAsync("ul.inventory.chest");
            if (storageElement is null)
            {
                _ = builder.WithStorage(CitizenStorage.Invisible);
            }
            else
            {
                IReadOnlyList<IElementHandle> itemsElement = await page.QuerySelectorAllAsync("ul.inventory.chest > li.item > span.item-icon > img[alt]");
                ImmutableArray<Item> items = (itemsElement ?? Enumerable.Empty<IElementHandle>())
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
        }

        private static async Task FetchDecoration(IPage page, CitizenEntryBuilder builder)
        {
            IElementHandle? decorationElement = await page.QuerySelectorAsync("div:nth-child(3) > div:nth-child(5) div:has(img)");
            string? decorationStr = decorationElement is null ? null : await decorationElement.TextContentAsync();
            _ = builder.WithDecoration(decorationStr.ConvertToDecoration());
        }
    }
}
