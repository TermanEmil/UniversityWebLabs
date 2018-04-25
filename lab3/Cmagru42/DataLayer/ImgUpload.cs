using System;
using System.Collections.Generic;

namespace DataLayer
{
    public class ImgUpload
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public byte[] RawImg { get; set; }
        public int Likes { get; set; }
    }
}
