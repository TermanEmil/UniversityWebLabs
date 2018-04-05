using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Account;
using BusinessLayer.Account.Models;
using DataLayer.DB;
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
        private readonly AccountManager _accountManager;
        private readonly IMapper _mapper;

        public AccountController(
            CmagruDBContext context,
            ILogger<AccountController> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _accountManager = new AccountManager(context, mapper);
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
                var regData = _mapper.Map<RegisterViewModel, RegistrationData>(model);

                var userRegistration = await _accountManager.UserRegister(regData);
                _logger.LogInformation("Registred: " + userRegistration.Status.ToString());
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
