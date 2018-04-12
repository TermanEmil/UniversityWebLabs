using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("")]
    [Route("Home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// An action to test authorization.
        /// </summary>
        [Authorize]
        [Route("AuthorizedIndex")]
        public IActionResult AuthorizedIndex(string url)
        {
            return View("Index");
        }
    }
}
