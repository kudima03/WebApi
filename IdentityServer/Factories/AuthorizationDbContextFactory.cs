using IdentityServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer.Factories
{
    public class AuthorizationDbContextFactory : IDesignTimeDbContextFactory<AuthorizationDbContext>
    {
        public AuthorizationDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthorizationDbContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("IdentityServer"));

            return new AuthorizationDbContext(optionsBuilder.Options);
        }
    }
}
