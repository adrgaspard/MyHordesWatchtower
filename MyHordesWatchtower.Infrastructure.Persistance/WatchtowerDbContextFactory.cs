using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyHordesWatchtower.Infrastructure.Persistance
{
    public class WatchtowerDbContextFactory : IDesignTimeDbContextFactory<WatchtowerDbContext>
    {
        public WatchtowerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("secrets.json")
                .Build();
            DbContextOptionsBuilder<WatchtowerDbContext> optionsBuilder = new();
            _ = optionsBuilder.UseNpgsql(configuration.GetValue<string>("Database:ConnectionString"));
            return new WatchtowerDbContext(optionsBuilder.Options);
        }
    }
}
