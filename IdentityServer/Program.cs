using IdentityServer;
using IdentityServer.Data;
using IdentityServer.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
            .MigrateDbContext<AuthorizationDbContext>((context, services) =>
            {
                var env = services.GetService<IWebHostEnvironment>();
                var logger = services.GetService<ILogger<AuthorizationDbContextSeed>>();

                new AuthorizationDbContextSeed()
                    .SeedAsync(context, env, logger, 10)
                    .Wait();
            })
            .MigrateDbContext<ConfigurationDbContext>((context, services) =>
            {
                var cfg = services.GetService<IConfiguration>();
                new ConfigurationDbContextSeed()
                    .SeedAsync(context, cfg)
                    .Wait();
            });
        host.Run();
    }

    public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}