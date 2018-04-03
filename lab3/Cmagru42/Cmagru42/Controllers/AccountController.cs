using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models.AccountViewModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
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
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                //var user 
            }
            else
            {
                // Go back if something failed.
                return View(model);   
            }
        }
    }
}
