using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHordesWatchtower.Application.Repositories;
using MyHordesWatchtower.Host;
using MyHordesWatchtower.Infrastructure.Persistance;
using MyHordesWatchtower.Infrastructure.Persistance.Repositories;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .AddJsonFile("secrets.json")
    .Build();

IServiceCollection services = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddDbContext<WatchtowerDbContext>(options =>
    {
        _ = options.UseNpgsql(configuration.GetValue<string>("Database:ConnectionString"));
    })
    .AddTransient<ICitizenEntryRepository, CitizenEntryRepository>()
    .AddLogging()
    .AddSingleton<Application>();


ServiceProvider serviceProvider = services.BuildServiceProvider();

serviceProvider.GetRequiredService<Application>().Start();