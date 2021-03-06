﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using BusinessLayer.Emailing;
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
        private readonly ImgControl _imgCtrl;

        public PhotoRoomController(
            ILogger<AccountController> logger,
            CmagruDBContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _imgCtrl = new ImgControl(_logger, _context, _userManager, emailService);
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
            await _imgCtrl.UploadImgFromRawStrAsync(data.RawImg, user);
            //return RedirectToAction("Index");
            return Json(new
            {
                success = true
            });
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
                    var err = await _imgCtrl.UploadImgFromPathAsync(
                        file.FileName,
                        memoryStream.ToArray(),
                        user);

                    if (err != null)
                    {
                        ViewData["UploadImgError"] = err;
                        return View("Index");
                    }
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
            var overlayerId = UserUtils.GetUserRoleId(_context, "ImgOverlayer");
            var imgs = _context.ImgUploads
                               .Where(x => x.UserId == overlayerId)
                               .Select(x => new OverlayImgViewModel
                               {
                                    ImgBase64 = x.RawImgToBase64()
                               }).ToList();
            return imgs;

        }
    }
}
