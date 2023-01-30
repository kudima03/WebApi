using IdentityServer.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityServer.Data
{
    public class ConfigurationDbContextSeed
    {
        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
        {
            var clientLinks = new Dictionary<string, string>()
            {
                { "MvcClient", configuration.GetValue<string>("MvcClient") },
            };

            if (!context.Clients.Any())
            {
                var clients = Config.GetClients(clientLinks);
                foreach (var client in clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                var apiScopes = Config.GetScopes();
                foreach (var scope in apiScopes)
                {
                    context.ApiScopes.Add(scope.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var api in Config.GetResources())
                {
                    context.ApiResources.Add(api.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
