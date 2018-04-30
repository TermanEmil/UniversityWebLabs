using System;
using System.Collections.Generic;

namespace DataLayer
{
    public class ImgUpload
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public byte[] RawImg { get; set; }

        public string RawImgToBase64()
        {
            return "data:image/png;base64," + Convert.ToBase64String(RawImg);
        }
    }
}
