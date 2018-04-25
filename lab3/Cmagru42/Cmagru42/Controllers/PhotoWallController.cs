using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
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
        // This number of images will be added if more imgs are required.
        private readonly int _deltaNbOfImgs = 20;

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
            if (model.Photos == null)
            {
                model = new GridPhotosViewModel()
                {
                    Photos = new List<PhotoViewModel>(),
                    FilterByLikes = 1
                };
                AddPhotosToModel(model);
            }
            _logger.LogInformation(model.Photos.Count.ToString());
            return View(model);
        }

        [HttpPost]
        [Route("GetPhotos")]
        public IActionResult GetPhotos([FromBody] GridPhotosViewModel model)
        {
            AddPhotosToModel(model);
            return RedirectToAction("Index", model);
        }

        private void AddPhotosToModel(GridPhotosViewModel model)
        {
            var imgs = _context.ImgUploads.Where(img =>
                                                 model.Photos.FirstOrDefault(
                                                     photo =>
                                                     photo.ImgId == img.Id) == null)
                               .Take(model.Photos.Count + _deltaNbOfImgs);

            model.Photos.AddRange(imgs.Select(img => new PhotoViewModel()
            {
                ImgId = img.Id,
                Likes = img.Likes,
                Base64Img = PhotoRoom.RawImgToBase64(img.RawImg),
                CommentsCount = _context.Comments.Where(c => c.ImgId == img.Id).Count()
            }));
        }
    }
}
