using System;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    public class Comment : IReaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ContentId { get; set; }
        public EReactionContentType ContentType { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PostTime { get; set; }

        [StringLength(
            256,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        public string Content { get; set; }
    }
}
