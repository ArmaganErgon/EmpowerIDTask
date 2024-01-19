namespace BlogService.Features.Posts.Model;

public readonly record struct CreateCommentRequest(long? PostId, string CreatedBy, string Content);
