using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHordesWatchtower.Application;
using MyHordesWatchtower.Application.Repositories;
using MyHordesWatchtower.Host;
using MyHordesWatchtower.Infrastructure.Persistance;
using MyHordesWatchtower.Infrastructure.Persistance.Repositories;
using MyHordesWatchtower.Infrastructure.RedisClient;
using MyHordesWatchtower.Infrastructure.WebClient;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .AddJsonFile("settings.json")
    .AddJsonFile("secrets.json")
    .Build();

IServiceCollection services = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<IPubSub, RedisClient>()
    .AddDbContext<WatchtowerDbContext>(options =>
    {
        _ = options.UseNpgsql(configuration.GetValue<string>("Database:ConnectionString"));
    })
    .AddTransient<ICitizenEntryRepository, CitizenEntryRepository>()
    .AddTransient<IWebClient, WebClient>()
    .AddLogging(options =>
    {
        _ = options.AddConfiguration(configuration.GetRequiredSection("Logging"));
        _ = options.AddConsole();
    })
    .AddSingleton<Application>();
ServiceProvider serviceProvider = services.BuildServiceProvider();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
serviceProvider.GetRequiredService<Application>().StartFarmThiefs();