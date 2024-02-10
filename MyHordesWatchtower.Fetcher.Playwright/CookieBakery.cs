using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHordesWatchtower.Fetcher.Playwright.Abstractions;
using MyHordesWatchtower.Fetcher.Playwright.Tools;
using System.Text.Json;

namespace MyHordesWatchtower.Fetcher.Playwright
{
    [TestClass]
    public partial class CookieBakery : ScriptRunner
    {
        public CookieBakery()
        {
            TimeoutInMs = 0;
            Headless = false;
            Locale = "fr-FR";
            AddAuthenticationCookies = false;
            EternalTwinUserLogin = Configuration.GetRequiredSection(Consts.EternalTwinUserLoginEnvVarName)?.Get<string>() ?? "";
            EternalTwinUserPassword = Configuration.GetRequiredSection(Consts.EternalTwinUserPasswordEnvVarName)?.Get<string>() ?? "";
        }
        private string EternalTwinUserLogin { get; set; }

        private string EternalTwinUserPassword { get; set; }

        [TestMethod]
        public async Task Script()
        {
            // Go to myhordes.eu
            IPage page = await Context.NewPageAsync();
            _ = await page.GotoAsync("https://myhordes.eu/jx/public/welcome");

            // Go to connection page
            await page.GetByRole(AriaRole.Link, new() { Name = "JOUER" }).ClickAsync();
            await page.GetByRole(AriaRole.Checkbox, new() { Name = "Rester connecté" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Connectez-vous via Eternaltwin" }).ClickAsync();
            await page.WaitForLoadStateAsync();
            await page.WaitForTimeoutAsync(2000);

            // Enter credentials and authenticate
            await page.GetByLabel("Nom d'utilisateur").ClickAsync();
            await page.GetByLabel("Nom d'utilisateur").FillAsync(EternalTwinUserLogin);
            await page.GetByLabel("Mot de passe").ClickAsync();
            await page.GetByLabel("Mot de passe").FillAsync(EternalTwinUserPassword);
            await page.WaitForTimeoutAsync(500);
            await page.GetByRole(AriaRole.Button, new() { Name = "Se connecter" }).ClickAsync();
            await page.WaitForTimeoutAsync(5000);

            // Fetch cookies and check if it's all good
            IReadOnlyList<BrowserContextCookiesResult>? cookies = await Context.CookiesAsync();
            string cookiesStr = cookies is null ? string.Empty : JsonSerializer.Serialize(cookies);
            Assert.IsFalse(string.IsNullOrWhiteSpace(cookiesStr));
            System.Diagnostics.Debug.WriteLine("Les cookies suivants ont été générés :");
            System.Diagnostics.Debug.WriteLine(cookiesStr);
            Assert.AreEqual(3, cookies?.Count ?? 0);
            Assert.IsTrue(cookies is not null && cookies.Any(c => c.Name == "sid" && c.Domain == "eternaltwin.org")
                && cookies.Any(c => c.Name == "myhordes_remember_me" && c.Domain == "myhordes.eu")
                && cookies.Any(c => c.Name == "myhordes_session_id" && c.Domain == "myhordes.eu"));
        }
    }
}
