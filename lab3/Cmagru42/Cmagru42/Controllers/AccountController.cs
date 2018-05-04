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
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                        email = "",
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
        public async Task<IActionResult> ConfirmEmail(string userId, string email, string code)
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
            if (result.Succeeded)
            {
                // Used to modify the current email.
                if (!string.IsNullOrEmpty(email))
                {
                    user.Email = email;
                    await TryUpdateModelAsync(user);
                    await _context.SaveChangesAsync();
                    await _userManager.UpdateAsync(user);
                    return View("EmailConfirmed");
                }
            }

            return View("Error");
        }

        [Route("Settings")]
        public IActionResult Settings()
        {
            return View(LoadSettinsViewModelData(new SettingsViewModel()));
        }

        [HttpPost]
        [Route("SetSettings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetSettings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var shouldUpdateDb = 0;
                var currentUser = await _userManager.GetUserAsync(User);

                shouldUpdateDb += await TryToModifUserSettings(model, currentUser);
                shouldUpdateDb += await TryToModifUsernameAsync(model, currentUser);
                shouldUpdateDb += await TryToModifEmailAsync(model, currentUser);

                if (shouldUpdateDb != 0)
                    await _context.SaveChangesAsync();
            }
            LoadSettinsViewModelData(model);
            return View("Settings", model);
        }

        [Route("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View("Settings", LoadSettinsViewModelData(new SettingsViewModel()));
        }

        [HttpPost]
        [Route("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (model.NewPassword != model.ConfirmPassword)
                    ModelState.AddModelError("", "Passwords don't match");
                else
                {
                    var result = await _userManager.ChangePasswordAsync(
                        user,
                        model.CurrentPassword,
                        model.NewPassword);
                    
                    if (result.Succeeded)
                    {
                        ViewBag.PasswordChanged = "Password successfully changed";
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View("Settings", LoadSettinsViewModelData(new SettingsViewModel()));
        }

        [Route("ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Such email is not registered.");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("PasswordRecovery", "Account", new
            {
                userid = user.Id,
                code = token
            }, protocol: HttpContext.Request.Scheme);

            try
            {
                await UserUtils.SendPasswordRecovery(
                    _emailService,
                    user.Email,
                    callbackUrl);
            }
            catch (Exception e)
            {
                ViewBag.RegistrationStatus = e.Message;
            }
            finally
            {
                ViewBag.RegistrationStatus = "Recovery link sent";
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("PasswordRecovery")]
        public async Task<IActionResult> PasswordRecovery(string userId, string code)
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

            TempData["PasswordRecoveryToken"] = code;
            TempData["PasswordRecoveryUserId"] = userId;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("PasswordRecovery")]
        public async Task<IActionResult> PasswordRecovery(PasswordRecoveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!TempData.ContainsKey("PasswordRecoveryToken") ||
                    !TempData.ContainsKey("PasswordRecoveryUserId"))
                {
                    ModelState.AddModelError("", "Internal TempData was deleted. Try again please.");
                    return View(model);
                }
                var user = await _userManager.FindByIdAsync(TempData["PasswordRecoveryUserId"] as string);
                var token = TempData["PasswordRecoveryToken"] as string;

                var rs = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (rs.Succeeded)
                    return RedirectToAction("Login");
                else
                    foreach (var error in rs.Errors)
                        ModelState.AddModelError("", error.Description);
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

        private async Task<int> TryToModifUserSettings(
            SettingsViewModel model,
            ApplicationUser currentUser)
        {
            var userId = currentUser.Id;
            var settings = _context.GetUserSettings
                                   .FirstOrDefault(x => x.UserId == userId);

            if (settings != null && settings.SendEmailNotifs == model.SendNotifsOnEmail)
                return 0;

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
            return 1;
        }

        private async Task<int> TryToModifUsernameAsync(
            SettingsViewModel model,
            ApplicationUser currentUser)
        {
            if (string.IsNullOrEmpty(model.NewUserName))
                return 0;

            var existingUser = await _userManager.FindByNameAsync(model.NewUserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "An user with the same UserName already exists.");
                return 0;
            }
            var rs = await _userManager.SetUserNameAsync(currentUser, model.NewUserName);
            if (rs.Succeeded)
            {
                await _userManager.UpdateAsync(currentUser);
                return 1;
            }
            else
            {
                foreach (var error in rs.Errors)
                    ModelState.AddModelError("", error.Description);
                return 0;
            }

        }

        private async Task<int> TryToModifEmailAsync(
            SettingsViewModel model,
            ApplicationUser currentUser)
        {
            if (string.IsNullOrEmpty(model.NewEmail))
                return 0;

            var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "An user with the same email already exists.");
                return 0;
            }

            var confirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
            var ctokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = currentUser.Id,
                email = model.NewEmail,
                code = confirmCode
            }, protocol: HttpContext.Request.Scheme);

            try
            {
                await UserUtils.SendEmailConfirm(
                    _emailService,
                    model.NewEmail,
                    ctokenLink);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            finally
            {
                ViewBag.Status = "Confirmation sent";
            }

            return 0;
        }

        private SettingsViewModel LoadSettinsViewModelData(SettingsViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Find(userId);
            var settings = _context.GetUserSettings
                                   .FirstOrDefault(x => x.UserId == userId);

            var sendNotifs = settings == null ? false : settings.SendEmailNotifs;

            model.SendNotifsOnEmail = sendNotifs;
            model.CurrentEmail = user.Email;
            model.CurrentUserName = user.UserName;
            return model;
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
