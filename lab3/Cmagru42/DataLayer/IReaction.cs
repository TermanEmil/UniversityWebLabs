using System;
namespace DataLayer
{
    public enum EReactionContentType
    {
        Img,
        Comment
    }

    public interface IReaction
    {
        string Id { get; set; }
        string UserId { get; set; }
        string ContentId { get; set; }
        EReactionContentType ContentType { get;set;}
    }
}
