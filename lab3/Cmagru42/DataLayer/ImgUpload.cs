using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    public class ImgUpload
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public byte[] RawImg { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UploadTime { get; set; }

        public string RawImgToBase64()
        {
            return "data:image/png;base64," + Convert.ToBase64String(RawImg);
        }
    }
}
