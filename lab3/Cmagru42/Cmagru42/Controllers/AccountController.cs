using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer;
using BusinessLayer.Emailing;
using DataLayer;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Extensions;
using Presentation.Models.AccountViewModels;
using Presentation.Services;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly CmagruDBContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            CmagruDBContext context,
            ILogger<AccountController> logger,
            IMapper mapper,
            IEmailService emailService)
        {
            System.Security.Claims.ClaimsPrincipal a = User;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _context = context;
        }

        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl = null)
        {
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
                if (!desiredUser.User.EmailConfirmed)
                    ModelState.AddModelError("", "Email not confirmed.");
                else
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
                    var confirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var ctokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        code = confirmCode
                    }, protocol: HttpContext.Request.Scheme);

                    try
                    {
                        await UserUtils.SendEmailConfirm(
                            _emailService,
                            user.Email,
                            ctokenLink);
                    }
                    catch (Exception e)
                    {
                        ViewBag.RegistrationStatus = e.Message;
                    }
                    finally
                    {
                        ViewBag.RegistrationStatus = "Confirmation sent";
                    }

                    return View(model);
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogInformation("Not found: userid = " + userId + "; code = " + code);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "EmailConfirmed" : "Error");
        }

        [Route("Settings")]
        public IActionResult Settings()
        {
            var userId = _userManager.GetUserId(User);
            var settings = _context.GetUserSettings
                                   .FirstOrDefault(x => x.UserId == userId);

            var sendNotifs = settings == null ? false : settings.SendEmailNotifs;

            var model = new SettingsViewModel()
            {
                SendNotifsOnEmail = sendNotifs
            };
            return View(model);
        }

        [HttpPost]
        [Route("SetSettings")]
        public async Task<IActionResult> SetSettings(SettingsViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var settings = _context.GetUserSettings
                                   .FirstOrDefault(x => x.UserId == userId);
            if (settings != null && settings.SendEmailNotifs == model.SendNotifsOnEmail)
                return View("Settings", model);

            if (settings == null)
            {
                settings = new UserSettings
                {
                    UserId = userId,
                    SendEmailNotifs = model.SendNotifsOnEmail
                };
                _context.GetUserSettings.Add(settings);
            }
            else
            {
                settings.SendEmailNotifs = model.SendNotifsOnEmail;
                await TryUpdateModelAsync(settings);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Settings");
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
