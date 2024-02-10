using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using MyHordesWatchtower.Application;
using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Infrastructure.WebClient
{
    public sealed partial class WebClient : IWebClient
    {
        private readonly IConfiguration _configuration;
        private readonly bool _headless;

        public WebClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _headless = configuration.GetValue<bool>("WebClient:Headless");
        }

        public async Task<IReadOnlyList<CitizenEntry>> CollectCitizensEntries()
        {
            return await ExecuteWithPlaywright(CollectCitizensEntries);
        }

        public async Task<string> GenerateAuthenticationCookies()
        {
            return await ExecuteWithPlaywright(GenerateAuthenticationCookies);
        }

        private async Task<TResult> ExecuteWithPlaywright<TResult>(Func<IBrowserContext, Task<TResult>> taskFunction)
        {
            IPlaywright playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Chromium.LaunchAsync(new() { Timeout = 0, Headless = _headless });
            IBrowserContext context = await browser.NewContextAsync(new() { Locale = "fr-FR" });
            Task<TResult> task = taskFunction(context);
            task.Wait();
            await context.CloseAsync();
            playwright.Dispose();
            return task.Result;
        }
    }
}
