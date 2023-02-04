using BooksAPI.Extensions;
using BooksAPI.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.MigrateDbContext<BooksContext>((context, services) =>
            {
                var env = services.GetService<IWebHostEnvironment>();
                var logger = services.GetService<ILogger<BooksContextSeed>>();
                new BooksContextSeed().SeedAsync(context, env, logger).Wait();
            });
            host.Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseWebRoot("Pictures");
    }
}
