using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using BusinessLayer.Emailing;
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
        private readonly ImgControl _imgCtrl;

        public PhotoWallController(
            ILogger<AccountController> logger,
            CmagruDBContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _imgCtrl = new ImgControl(logger, context, userManager, emailService);
        }

        [Route(""), Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("GetNewImgs")]
        public async Task<JsonResult> GetNewImgs([FromBody] GridPhotosViewModel model)
        {
            var rsImgs = _imgCtrl.GetNewImgs(
                model.DisplayedImgIds,
                model.RequiredImgs,
                await _userManager.GetUserAsync(User));
            
            if (rsImgs == null)
                return Json(new
                {
                    success = false
                });

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

            var likesCount = await _imgCtrl.LikeImg(user, model.ContentId);

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
            await _imgCtrl.PostComment(user, model.ImgId, model.Content);

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
        [Route("GetImgOwnerData/{imgId}")]
        public async Task<JsonResult> GetImgOwnerData([FromRoute] string imgId)
        {
            var _permissions = new List<string>();

            if (!User.Identity.IsAuthenticated)
                return Json(new
                {
                    success = true,
                    permissions = _permissions
                });

            var user = await _userManager.GetUserAsync(User);
            var img = _context.ImgUploads.FirstOrDefault(x => x.Id == imgId);

            if (img == null)
                return Json(new
                {
                    success = false,
                    error = "No such img"
                });

            if (img.UserId == user.Id || UserUtils.GetUserRole(_context, user) == "Admin")
                _permissions.Add("Write");

            return Json(new
            {
                success = true,
                permissions = _permissions,
                owner = _context.Users.FirstOrDefault(x => x.Id == img.UserId)?.UserName
            });
        }

        [HttpPost]
        [Authorize]
        [Route("RemoveImg/{imgId}")]
        public async Task<JsonResult> RemoveImg([FromRoute] string imgId)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                await _imgCtrl.RemoveImg(user, imgId);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    error = e.Message
                });
            }

            return Json(new
            {
                success = true,
                redirUrl = GetAbsPathOf(Url.Action("Index"))
            });
        }

        private string GetAbsPathOf(string localPath)
        {
            var routeUrl = localPath;
            var absUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, routeUrl);
            var uri = new Uri(absUrl, UriKind.Absolute);
            return uri.ToString();
        }
    }
}
