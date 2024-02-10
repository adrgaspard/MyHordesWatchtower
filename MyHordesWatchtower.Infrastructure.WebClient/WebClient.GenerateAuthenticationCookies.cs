using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using MyHordesWatchtower.Application;
using System.Text.Json;

namespace MyHordesWatchtower.Infrastructure.WebClient
{
    public sealed partial class WebClient : IWebClient
    {
        private async Task<string> GenerateAuthenticationCookies(IBrowserContext context)
        {
            string eternalTwinLogin = _configuration.GetValue<string>("WebClient:Authentication:EternalTwinLogin") ?? "";
            string eternalTwinPassword = _configuration.GetValue<string>("WebClient:Authentication:EternalTwinPassword") ?? "";

            // Go to myhordes.eu
            IPage page = await context.NewPageAsync();
            _ = await page.GotoAsync("https://myhordes.eu/jx/public/welcome");

            // Go to connection page
            await page.GetByRole(AriaRole.Link, new() { Name = "JOUER" }).ClickAsync();
            await page.GetByRole(AriaRole.Checkbox, new() { Name = "Rester connecté" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Connectez-vous via Eternaltwin" }).ClickAsync();
            await page.WaitForLoadStateAsync();
            await page.WaitForTimeoutAsync(2000);

            // Enter credentials and authenticate
            await page.GetByLabel("Nom d'utilisateur").ClickAsync();
            await page.GetByLabel("Nom d'utilisateur").FillAsync(eternalTwinLogin);
            await page.GetByLabel("Mot de passe").ClickAsync();
            await page.GetByLabel("Mot de passe").FillAsync(eternalTwinPassword);
            await page.WaitForTimeoutAsync(500);
            await page.GetByRole(AriaRole.Button, new() { Name = "Se connecter" }).ClickAsync();
            await page.WaitForTimeoutAsync(5000);

            // Fetch cookies
            IReadOnlyList<BrowserContextCookiesResult>? cookies = await context.CookiesAsync();
            return cookies is null ? string.Empty : JsonSerializer.Serialize(cookies);
        }
    }
}
