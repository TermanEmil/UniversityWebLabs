using System;
namespace Presentation.Models.PhotoWallViewModels
{
    public class PhotoViewModel
    {
        public string Base64Img { get; set; }
        public string ImgId { get; set; }
        public int Likes { get; set; }
        public int CommentsCount { get; set; }
    }
}
