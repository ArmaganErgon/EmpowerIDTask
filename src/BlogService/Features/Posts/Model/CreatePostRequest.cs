namespace BlogService.Features.Posts.Model;

public readonly record struct CreatePostRequest(string CreatedBy, string Title, string Content);
