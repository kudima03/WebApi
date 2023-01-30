using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService<ApplicationUser> _loginService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        public AccountController(
            ILoginService<ApplicationUser> loginService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _loginService = loginService;
            _interaction = interaction;
            _clientStore = clientStore;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var authorizationContext = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var vm = BuildLoginViewModel(returnUrl, authorizationContext);
            ViewData["ReturnUrl"] = returnUrl;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _loginService.FindByUsername(model.Email);
                if (await _loginService.ValidateCredentials(user, model.Password))
                {
                    var tokenLifeTime = _configuration.GetValue("TokenLifetimeMinutes", 120);
                    var properties = new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(tokenLifeTime),
                        AllowRefresh = true,
                        RedirectUri = model.ReturnUrl,
                        IsPersistent = false
                    };
                    await _loginService.SignInAsync(user, properties);
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return Redirect("~/");
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModel(model);

            ViewData["ReturnUrl"] = model.ReturnUrl;

            return View(vm);
        }


        private LoginViewModel BuildLoginViewModel(string returnUrl, AuthorizationRequest authorizationContext)
        {
            return new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                Email = authorizationContext.LoginHint
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModel(LoginViewModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            var vm = BuildLoginViewModel(model.ReturnUrl, context);
            vm.Email = model.Email;
            return vm;
        }

    }
}
