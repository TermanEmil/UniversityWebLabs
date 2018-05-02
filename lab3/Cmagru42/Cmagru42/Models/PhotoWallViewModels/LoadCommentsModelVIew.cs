using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.PhotoWallViewModels
{
    public class LoadCommentsModelVIew
    {
        [Required]
        public string ImgId { get; set; }
    }
}
