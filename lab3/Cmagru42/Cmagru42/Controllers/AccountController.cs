using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Account;
using BusinessLayer.Account.Models;
using DataLayer.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models.AccountViewModels;
using Presentation.Controllers.Extensions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using DataLayer.AppUser;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("Account")]
    public class AccountController : Controller
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly AccountManager _accountManager;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            CmagruDBContext context,
            ILogger<AccountController> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _accountManager = new AccountManager(context, mapper);
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl = null)
        {
            _logger.LogInformation("MyIdentity: " + _signInManager.IsSignedIn(User));// User.Identity.IsAuthenticated);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            _logger.LogInformation("Login: POST");

            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var desiredUser = await GetDesiredUser(model.EmailOrUsername);

            if (!desiredUser.Succeded)
            {
                ModelState.AddModelError("", desiredUser.ErrorMsg);
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                desiredUser.User.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);   
            }
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return this.RedirectToActionCtrl(
                nameof(HomeController.Index),
                nameof(HomeController));
        }

        [Route("Register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Register: POST");
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<ApplicationUser>(model);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return this.RedirectToActionCtrl(
                        nameof(HomeController.Index),
                        nameof(HomeController));
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        #region Helpers
        private async Task<_DesiredLoginUserResult> GetDesiredUser(string emailOrUsername)
        {
            string errorMsg = null;
            ApplicationUser desiredUser;

            if (emailOrUsername.Contains("@"))
            {
                desiredUser = await _userManager.FindByEmailAsync(
                    emailOrUsername.Normalize());
                errorMsg = "No such email.";
            }
            else
            {
                desiredUser = await _userManager.FindByNameAsync(
                    emailOrUsername);
                errorMsg = "No such UserName";
            }

            return new _DesiredLoginUserResult()
            {
                User = desiredUser,
                ErrorMsg = errorMsg
            };
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }

    class _DesiredLoginUserResult
    {
        public bool Succeded
        {
            get
            {
                return this.User != null;
            }
        }

        public IdentityUser User { get; set; }
        public string ErrorMsg { get; set; }
    }
}
