using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Extensions;

namespace Presentation.Controllers
{
    [Route("Photoroom")]
    public class PhotoRoomController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            else
                return this.RedirectToActionCtrl(nameof(AccountController.Login), nameof(AccountController));
        }
    }
}
