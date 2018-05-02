using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.PhotoWallViewModels
{
    public class GetImgOwnerPermissionsViewModel
    {
        [Required]
        public string ImgId { get; set; }
    }
}
