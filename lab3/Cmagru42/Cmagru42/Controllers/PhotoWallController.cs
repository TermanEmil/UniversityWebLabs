using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("GetNewImgs")]
        public JsonResult GetNewImgs([FromBody] GridPhotosViewModel model)
        {
            var imgOverlayer = _context.Users.FirstOrDefault(x => x.UserName == "ImgOverlayer");
            var imgOverlayerId = imgOverlayer?.Id;

            var querry = _context.ImgUploads
                                 .Where(img =>
                                        !model.DisplayedImgIds.Contains(img.Id) &&
                                        img.UserId != imgOverlayerId);

            if (querry.Count() <= model.RequiredImgs[0])
                return Json(new
                {
                    success = false
                });
            
            var rsImgs = querry.OrderByDescending(x => x.UploadTime)
                               .Skip(model.RequiredImgs[0])
                               .Take(model.RequiredImgs.Count);
            
            var imgResponses = rsImgs.Select(img => new
            {
                imgId = img.Id,
                imgBase64 = img.RawImgToBase64(),
                likes = _context.Likes.Count(x => x.ContentId == img.Id),
                comments = _context.Comments.Count(x => x.ContentId == img.Id)
            });

            return Json(new
            {
                success = true,
                imgs = imgResponses
            });
        }

        [Route("LikeImg")]
        [Authorize]
        public async Task<JsonResult> LikeImg([FromBody] LikeContentModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new
                {
                    success = false
                });
            }

            var prevLike = _context.Likes.FirstOrDefault(x =>
                                                         x.UserId == user.Id &&
                                                         x.ContentId == model.ContentId);

            if (prevLike == null)
            {
                var like = new Like()
                {
                    UserId = user.Id,
                    ContentId = model.ContentId,
                    ContentType = EReactionContentType.Img
                };
                await _context.Likes.AddAsync(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Likes.Remove(prevLike);
                await _context.SaveChangesAsync();
            }

            var likesCount = _context.Likes.Count(x => x.ContentId == model.ContentId);
            return Json(new
            {
                success = true,
                currentLikes = likesCount
            });
        }

        [HttpPost]
        [Authorize]
        [Route("PostComment")]
        public async Task<JsonResult> PostComment([FromBody] PostCommentViewModel model)
        {
            if (model.Content.Length <= 1)
                return Json(new
                {
                    success = false,
                    error = "Comment too short"
                });

            var user = await _userManager.GetUserAsync(User);
            var comment = new Comment()
            {
                UserId = user.Id,
                ContentId = model.ImgId,
                PostTime = DateTime.Now,
                ContentType = EReactionContentType.Img,
                Content = model.Content
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        [Route("GetComments")]
        public JsonResult GetComments([FromBody] LoadCommentsModelVIew model)
        {
            var comments = _context.Comments
                                   .Where(c => c.ContentId == model.ImgId)
                                   .OrderBy(c => c.PostTime)
                                   .Select(c => new
                                   {
                                       content = c.Content,
                                       user = _context.Users.FirstOrDefault(u => u.Id == c.UserId).UserName,
                                       time = c.PostTime
                                    });
            return Json(new
            {
                success = true,
                allComments = comments
            });
        }

        [HttpPost]
        [Route("GetImgOwnerPermissions")]
        public JsonResult GetImgOwnerPermissions(string imgId)
        {
            _logger.LogInformation("ImgId: " + imgId);
            return Json(new
            {
                success = true
            });
        }
    }
}
