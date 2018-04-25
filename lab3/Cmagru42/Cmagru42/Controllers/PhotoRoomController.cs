using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models.PhotoRoomViewModels;

namespace Presentation.Controllers
{
    [Route("Photoroom")]
    [Authorize]
    public class PhotoRoomController : Controller
    {
        private readonly ILogger _logger;
        private readonly CmagruDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PhotoRoom _photoRoom;

        public PhotoRoomController(
            ILogger<AccountController> logger,
            CmagruDBContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _photoRoom = new PhotoRoom(_logger, _context, _userManager);
        }

        [Route("Index"), Route("")]
        public IActionResult Index()
        {
            var overlays = GetOverlaysImgs();
            return View(overlays);
        }

        [HttpPost]
        [Route("UploadImgRaw")]
        public async Task<IActionResult> UploadImgRaw([FromBody] ImgUploadRawViewModel data)
        {
            var user = await GetCurrentUserAsync();
            await _photoRoom.UploadImgFromRawStrAsync(data.RawImg, user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("UploadImgPath")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImgPath(IList<IFormFile> files)
        {
            _logger.LogInformation("UploadFromPath!");
            var user = await GetCurrentUserAsync();

            if (files.Count == 0)
                return RedirectToAction("Index");

            var file = files.First();

            if (file.Length >= Math.Pow(2, 10 + 10))
            {
                ViewData["UploadImgError"] = "File too large";
                return RedirectToAction("Index");
            }

            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var err = await _photoRoom.UploadImgFromPathAsync(
                        file.FileName,
                        memoryStream.ToArray(),
                        user);

                    if (err != null)
                        ViewData["UploadImgError"] = err;
                }
            }

            return RedirectToAction("Index");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        private IList<OverlayImgViewModel> GetOverlaysImgs()
        {
            var overlayerDB = _context.Users.FirstOrDefault(
                x => x.UserName.Equals("ImgOverlayer"));

            if (overlayerDB == null)
                return null;

            var imgs = _context.ImgUploads
                               .Where(x => x.UserId.Equals(overlayerDB.Id))
                               .Select(x => new OverlayImgViewModel
                               {
                                    ImgBase64 = PhotoRoom.RawImgToBase64(x.RawImg)
                               }).ToList();
            return imgs;

        }
    }
}
