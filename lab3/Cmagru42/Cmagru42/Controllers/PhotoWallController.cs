using System.Collections.Generic;
using System.Linq;
using DataLayer;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models.PhotoWallViewModels;

namespace Presentation.Controllers
{
    [Route("PhotoWall")]
    public class PhotoWallController : Controller
    {
        private readonly ILogger _logger;
        private readonly CmagruDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PhotoWallController(
            ILogger<AccountController> logger,
            CmagruDBContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Route(""), Route("Index")]
        public IActionResult Index(GridPhotosViewModel model)
        {
            //if (model.Photos == null)
            //{
            //    model = new GridPhotosViewModel()
            //    {
            //        Photos = new List<PhotoViewModel>(),
            //        FilterByLikes = 1
            //    };
            //    AddPhotosToModel(model);
            //}
            //_logger.LogInformation(model.Photos.Count.ToString());
            return View(model);
        }

        [HttpPost]
        [Route("GetNewImg")]
        public JsonResult GetNewImg([FromBody] GridPhotosViewModel model)
        {
            var querry = _context.ImgUploads
                                 .Where(img => !model.DisplayedImgIds.Contains(img.Id))
                                 .OrderByDescending(x => x.Likes);

            if (querry.Count() <= model.RequiredImg)
                return Json(new
                {
                    success = false,
                    imgNb = model.RequiredImg,
                });
            
            var rsImg = querry.Skip(model.RequiredImg).First();
            int commentsCount = _context.Comments.Count(c => c.ContentId == rsImg.Id);

            return Json(new
            {
                success = true,
                imgNb = model.RequiredImg,
                imgId = rsImg.Id,
                imgBase64 = rsImg.RawImgToBase64(),
                likes = rsImg.Likes,
                comments = commentsCount
            });
        }
    }
}
