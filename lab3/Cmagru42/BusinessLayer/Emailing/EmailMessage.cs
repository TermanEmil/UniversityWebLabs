using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace BusinessLayer.Emailing
{
    public class EmailMessage
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsHtml { get; set; } = true;
    }
}
