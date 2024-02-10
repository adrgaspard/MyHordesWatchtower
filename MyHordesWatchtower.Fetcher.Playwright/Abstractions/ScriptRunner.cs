using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHordesWatchtower.Fetcher.Playwright.Tools;

namespace MyHordesWatchtower.Fetcher.Playwright.Abstractions
{
    [TestClass]
    public abstract class ScriptRunner
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        protected ScriptRunner()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<ScriptRunner>()
                .Build();
        }

        protected IConfiguration Configuration { get; set; }

        protected IPlaywright Playwright { get; set; }

        protected IBrowser Browser { get; set; }

        protected IBrowserContext Context { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        protected float? TimeoutInMs { get; set; }

        protected bool? Headless { get; set; }

        protected string? Locale { get; set; }

        protected bool? AddAuthenticationCookies { get; set; }

        [TestInitialize]
        public virtual async Task Startup()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new() { Timeout = TimeoutInMs, Headless = Headless });
            Context = await Browser.NewContextAsync(new() { Locale = Locale });
            if (AddAuthenticationCookies is true)
            {
                await Context.AddAuthenticationCookiesAsync(Configuration);
            }
        }

        [TestCleanup]
        public virtual async Task Cleanup()
        {
            if (Context is not null)
            {
                await Context.CloseAsync();
            }
            Playwright?.Dispose();
        }
    }
}
