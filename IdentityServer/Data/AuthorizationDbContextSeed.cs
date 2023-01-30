using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Data
{
    public class AuthorizationDbContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(AuthorizationDbContext context, IWebHostEnvironment environment,
            ILogger<AuthorizationDbContextSeed> logger, int retry = 0)
        {
            try
            {
                if (!context.Users.Any())
                {
                    await context.AddRangeAsync(GetDefaultUsers());
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retry < 5)
                {
                    retry++;
                    logger.LogError(ex, "Exception occured while migrating", nameof(AuthorizationDbContextSeed));
                    await SeedAsync(context, environment, logger, retry);
                }
            }
        }

        private List<ApplicationUser> GetDefaultUsers()
        {
            var user = new ApplicationUser()
            {
                CardHolderName = "Cardholder",
                CardNumber = "4017111111111881",
                CardType = 1,
                City = "Minsk",
                Country = "Belarus",
                Email = "demouser@gmail.com",
                Expiration = "12/25",
                Id = Guid.NewGuid().ToString(),
                LastName = "LastName",
                Name = "User",
                PhoneNumber = "1234567890",
                UserName = "demouser@gmail.com",
                Street = "Central st.",
                SecurityNumber = "535",
                NormalizedEmail = "DEMOUSER@GMAIL.COM",
                NormalizedUserName = "DEMOUSER@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "demopassword");

            return new List<ApplicationUser> { user };
        }
    }
}
