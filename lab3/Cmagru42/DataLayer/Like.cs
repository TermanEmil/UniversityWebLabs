using System;
namespace DataLayer
{
    public class Like : IReaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ContentId { get; set; }
        public EReactionContentType ContentType { get; set; }
    }
}
