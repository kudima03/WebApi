using System.Security.Claims;

namespace WebMvcClient.Services
{
    public interface IIdentityParser<T>
    {
        T Parse(ClaimsPrincipal claimsPrincipal);
    }
}
