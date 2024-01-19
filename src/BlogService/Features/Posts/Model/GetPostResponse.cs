using BlogService.Features.Posts.Domain;

namespace BlogService.Features.Posts.Model;

public readonly record struct GetPostResponse(Post Post);