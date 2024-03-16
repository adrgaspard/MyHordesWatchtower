using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using MyHordesWatchtower.Application;
using MyHordesWatchtower.Infrastructure.WebClient.Events;
using MyHordesWatchtower.Infrastructure.WebClient.Tools;

namespace MyHordesWatchtower.Infrastructure.WebClient
{
    public sealed partial class WebClient : IWebClient
    {
        private async Task FarmThiefs(IBrowserContext context)
        {
            await context.AddAuthenticationCookiesAsync(_configuration);
            bool willStartStealing = _configuration.GetValue<bool>("WebClient:WillStartStealing");
            int itemsPerProcedure = _configuration.GetValue<int>("WebClient:ItemsPerStealProcedure");
            int selfId = _configuration.GetValue<int>("WebClient:FarmStealSelfHordesId");
            int partnerId = _configuration.GetValue<int>("WebClient:FarmStealPartnerHordesId");
            string farmStealChannel = _configuration.GetValue<string>("RedisClient:FarmStealChannel") ?? "";

            // Go to myhordes.eu
            IPage page = await context.NewPageAsync();
            _ = await page.GotoAsync("https://myhordes.eu");
            await page.WaitForRandomDuration(500, 1000);

            // Subscribe to stealing events
            TaskCompletionSource<bool> taskSource = new();
            if (!string.IsNullOrWhiteSpace(farmStealChannel))
            {
                await _pubSub.Subscribe(farmStealChannel, async (StealFinishedEvent @event) =>
                {
                    if (@event.ThiefHordesId == partnerId)
                    {
                        await GoInside(page);
                        await StealProcedure(page, partnerId, itemsPerProcedure);
                        await GoOutside(page);
                        await _pubSub.Publish(farmStealChannel, new StealFinishedEvent() { ThiefHordesId = selfId });
                    }
                });
                await _pubSub.Subscribe(farmStealChannel, async (PartnerReadyEvent @event) =>
                {
                    if (@event.PartnerHordesId == partnerId)
                    {
                        if (!taskSource.Task.IsCompleted)
                        {
                            _ = taskSource.TrySetResult(true);
                            if (!willStartStealing)
                            {
                                await GoOutside(page);
                                await _pubSub.Publish(farmStealChannel, new PartnerReadyEvent() { PartnerHordesId = selfId });
                            }
                        }
                    }
                });
                if (!taskSource.Task.IsCompleted)
                {
                    await _pubSub.Publish(farmStealChannel, new PartnerReadyEvent() { PartnerHordesId = selfId });
                }
            }

            // Wait for partner to be found and start stealing
            taskSource.Task.Wait();
            if (willStartStealing)
            {
                await StealProcedure(page, partnerId, itemsPerProcedure);
                await GoOutside(page);
                await _pubSub.Publish(farmStealChannel, new StealFinishedEvent() { ThiefHordesId = selfId });
            }

            await Task.Delay(TimeSpan.FromHours(12));
        }

        private async Task StealProcedure(IPage page, int partnerId, int itemsPerProcedure)
        {
            // Go to the citizens section
            await page.WaitForRandomDuration(500, 1000);
            await page.Locator(".row > div:nth-child(4) > div").First.ClickAsync();
            _ = await page.WaitForResponseAsync("https://myhordes.eu/jx/town/citizens");
            await page.WaitForLoadingFinishedAsync();

            // Go to the partner page
            await page.Locator($".userCell:has(a.username[x-user-id='{partnerId}'])").First.ClickAsync();
            await page.WaitForLoadingFinishedAsync();

            // Steal everything
            for (int i = 0; i < itemsPerProcedure; i++)
            {
                try
                {
                    bool dialogAccepted = false;
                    string objectName = i == 0 ? "Costume de Lutin Vert" : "Planche tordue";
                    async void OnPageDialog(object? sender, IDialog dialog)
                    {
                        await dialog.AcceptAsync();
                        page.Dialog -= OnPageDialog;
                        dialogAccepted = true;
                    }
                    page.Dialog += OnPageDialog;
                    await page.GetByRole(AriaRole.Button, new() { Name = "Entrer pour voler un objet" }).ClickAsync();
                    while (!dialogAccepted) ;
                    await page.Locator("#content").GetByRole(AriaRole.Img, new() { Name = objectName }).First.ClickAsync();
                    await page.WaitForLoadingFinishedAsync();
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Put all the stuff in the chest
            await page.Locator(".row > div[x-ajax-href='/jx/town/house/dash'] > div").First.ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Vider mon sac à dos dans le" }).ClickAsync();
            await page.WaitForLoadingFinishedAsync();
        }

        private async Task GoOutside(IPage page)
        {
            await page.Locator(".row > div:nth-child(6) > div").First.ClickAsync();
            await page.WaitForLoadingFinishedAsync();
            await page.Locator("#door_exit").First.ClickAsync();
            await page.WaitForLoadingFinishedAsync();
        }

        private async Task GoInside(IPage page)
        {
            await page.Locator("#town-enter").First.ClickAsync();
            await page.WaitForLoadingFinishedAsync();
        }
    }
}

