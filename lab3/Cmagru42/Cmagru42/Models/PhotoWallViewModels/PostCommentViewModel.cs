using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.PhotoWallViewModels
{
    public class PostCommentViewModel
    {
        public string ImgId { get; set; }

        [StringLength(
            124,
            MinimumLength = 1,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Content { get; set; }
    }
}
