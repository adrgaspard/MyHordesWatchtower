﻿using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace MyHordesWatchtower.Fetcher.Playwright.Tools
{
    public static class PlaywrightExtensions
    {
        public static async Task AddAuthenticationCookiesAsync(this IBrowserContext context, IConfiguration configuration)
        {
            IConfigurationSection? cookiesSection = configuration.GetRequiredSection(Consts.AuthenticationCookiesEnvVarName);
            if (cookiesSection is null)
            {
                return;
            }
            Cookie[] cookies = cookiesSection.Get<Cookie[]>() ?? [];
            if (cookies.Length == 0)
            {
                return;
            }
            await context.AddCookiesAsync(cookies);
        }

        public static async Task WaitForRandomDuration(this IPage page, float minInMs, float maxInMs)
        {
            if (minInMs < 0)
            {
                throw new ArgumentException("This parameter can't be lower or equal to 0", nameof(minInMs));
            }
            if (minInMs > maxInMs)
            {
                throw new ArgumentException($"{nameof(minInMs)} can't be greater than {nameof(maxInMs)}!");
            }
            float delay = (float)((double)minInMs + (((double)maxInMs - minInMs) * Random.Shared.NextDouble()));
            if (delay > 0)
            {
                await page.WaitForTimeoutAsync(delay);
            }
        }

        public static async Task WaitForLoadingFinishedAsync(this IPage page)
        {
            _ = await page.WaitForSelectorAsync("#loadzone[x-stack='1']");
            _ = await page.WaitForSelectorAsync("#loadzone[x-stack='0']");
        }
    }
}