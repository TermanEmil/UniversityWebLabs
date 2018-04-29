using System;
using System.Collections.Generic;

namespace Presentation.Models.PhotoWallViewModels
{
    public class GridPhotosViewModel
    {
        public List<string> DisplayedImgIds { get; set; }
        public int RequiredImg { get; set; }
        public string FilterByEmail { get; set; }
        public string FilterByUserName { get; set; }

        //  0 if no
        //  1 if most popular first
        // -1 if most popular last
        public int FilterByLikes { get; set; }
        public int FilterByComments { get; set; }
    }
}
