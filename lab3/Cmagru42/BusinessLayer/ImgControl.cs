using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.Emailing;
using DataLayer;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BusinessLayer
{
    public class ImgControl
    {
        private readonly ILogger _logger;
        private readonly CmagruDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public static readonly IList<string> imageExtensions = new List<string>
        {
            ".JPG", ".JPE", ".BMP", ".PNG", ".JPEG"
        };


        public ImgControl(
            ILogger logger,
            CmagruDBContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
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
                return "Not a supported format";

            await UplodImgAsync(imgBytes, user);
            return null;
        }

        public IEnumerable<ImgUpload> GetNewImgs(
            IList<string> exceptionImgIds,
            IList<int> requiredImgs,
            ApplicationUser loggedInUser)
        {
            string overlayerId = UserUtils.GetUserRoleId(_context, "ImgOverlayer");
            string[] specialRoles =
            {
                "ImgOverlayer",
                "Admin",
            };

            if (loggedInUser != null && specialRoles.Contains(UserUtils.GetUserRole(_context, loggedInUser)))
                overlayerId = "It's img overlayer, so show me overlayer imgs too.";
            
            var querry = _context.ImgUploads
                                 .Where(img =>
                                        !exceptionImgIds.Contains(img.Id) &&
                                        img.UserId != overlayerId);

            if (querry.Count() <= requiredImgs[0])
                return null;

            var rsImgs = querry.OrderByDescending(x => x.UploadTime)
                               .Skip(requiredImgs[0])
                               .Take(requiredImgs.Count);
            
            return rsImgs;
        }

        public async Task<int> LikeImg(ApplicationUser currentUser, string likeContentId)
        {
            var prevLike = _context.Likes.FirstOrDefault(x =>
                                                         x.UserId == currentUser.Id &&
                                                         x.ContentId == likeContentId);

            if (prevLike == null)
            {
                var like = new Like()
                {
                    UserId = currentUser.Id,
                    ContentId = likeContentId,
                    ContentType = EReactionContentType.Img
                };
                await _context.Likes.AddAsync(like);

            }
            else
                _context.Likes.Remove(prevLike);
            
            await _context.SaveChangesAsync();

            var likesCount = _context.Likes.Count(x => x.ContentId == likeContentId);
            return likesCount;
        }

        public async Task PostComment(
            ApplicationUser user,
            string imgId,
            string content)
        {
            var comment = new Comment()
            {
                UserId = user.Id,
                ContentId = imgId,
                PostTime = DateTime.Now,
                ContentType = EReactionContentType.Img,
                Content = content
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var img = _context.ImgUploads.Find(imgId);
            if (img == null)
                return;
            if (img.UserId == user.Id)
                return;

            var subject = "[Cmagru][Commented][no-reply] " + user.UserName + " commented";
            var body = "<b>" + user.UserName + "</b> ";
            body += "has just submited a comment: <br />";
            body += "<i>" + content + "</i>";

            if (user.EmailConfirmed)
                body += "<br /> Send him a reply: " + user.Email;

            UserUtils.SendEmailNotif(
                _context,
                _emailService,
                img.UserId,
                subject,
                body
            );
        }

        public async Task RemoveImg(ApplicationUser user, string imgId)
        {
            var img = _context.ImgUploads.FirstOrDefault(x => x.Id == imgId);
            if (img == null)
                throw new Exception("No such img");

            if (img.UserId != user.Id && UserUtils.GetUserRole(_context, user) != "Admin")
                throw new Exception("Action not permited");

            RemoveImg(img);
            await _context.SaveChangesAsync();
        }

        private async Task UplodImgAsync(byte[] imgBytes, ApplicationUser user)
        {
            var imgUpload = new ImgUpload()
            {
                RawImg = imgBytes,
                UserId = user.Id,
                UploadTime = DateTime.Now
            };

            var result = await _context.ImgUploads.AddAsync(imgUpload);
            await _context.SaveChangesAsync();
        }

        private void RemoveImg(ImgUpload img)
        {
            _context.ImgUploads.Remove(img);
            _context.Comments.RemoveRange(_context.Comments.Where(c => c.ContentId == img.Id));
            _context.Likes.RemoveRange(_context.Likes.Where(l => l.ContentId == img.Id));
        }
    }
}
