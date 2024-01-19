namespace TestApp;

public readonly record struct CreateCommentRequest(long? PostId, string CreatedBy, string Content);

public readonly record struct CreateCommentResponse(long Id);

public readonly record struct CreatePostRequest(string CreatedBy, string Title, string Content);

public readonly record struct CreatePostResponse(long Id);

public readonly record struct GetPostRequest(long Id);

public readonly record struct GetPostResponse(Post Post);

public class Comment
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public string CreatedBy { get; set; }
    public string Content { get; set; }
}

public class Post
{
    public long Id { get; set; }
    public string CreatedBy { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public ICollection<Comment>? Comments { get; set; }
}
