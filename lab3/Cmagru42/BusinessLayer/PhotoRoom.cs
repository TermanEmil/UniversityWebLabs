using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BusinessLayer
{
    public class PhotoRoom
    {
        private readonly ILogger _logger;
        private readonly CmagruDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public static readonly IList<string> imageExtensions = new List<string>
        {
            ".JPG", ".JPE", ".BMP", ".PNG"
        };


        public PhotoRoom(
            ILogger logger,
            CmagruDBContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task UploadImgFromRawStrAsync(
            string imgBase64,
            ApplicationUser user)
        {
            // Remove 'data:image/png;base64,'
            var rawImgNoHeader = imgBase64.Remove(0, imgBase64.IndexOf(',') + 1);
            var rawImg = Convert.FromBase64String(rawImgNoHeader);

            await UplodImgAsync(rawImg, user);
        }

        public async Task<string> UploadImgFromPathAsync(
            string imgName,
            byte[] imgBytes,
            ApplicationUser user)
        {
            imgName = imgName.ToUpper();
            if (!imageExtensions.Contains(Path.GetExtension(imgName)))
                return "Not an image";
            
            await UplodImgAsync(imgBytes, user);
            return null;
        }

        private async Task UplodImgAsync(byte[] imgBytes, ApplicationUser user)
        {
            var imgUpload = new ImgUpload()
            {
                RawImg = imgBytes,
                UserId = user.Id
            };

            var result = await _context.ImgUploads.AddAsync(imgUpload);
            await _context.SaveChangesAsync();
        }

        public static string RawImgToBase64(byte[] rawImg)
        {
            return "data:image/png;base64," + Convert.ToBase64String(rawImg);
        }
    }
}
