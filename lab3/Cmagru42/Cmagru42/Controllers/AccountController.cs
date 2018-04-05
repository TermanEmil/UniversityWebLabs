using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models.AccountViewModels;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Register")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            System.Diagnostics.Debug.WriteLine(ViewData);
            _logger.LogInformation("Register1");
            return View();
        }

        [Route("Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model,
            string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            _logger.LogInformation("Register2");
            if (ModelState.IsValid)
            {
                //var user = new ApplicationUser
                //{
                //    UserName = model.UserName,
                //    Email = model.Email,
                //    Password = model.Password
                //};

            }
            else
            {
                // Go back if something failed.
                return View(model);   
            }
            return View(model);
        }
    }
}
