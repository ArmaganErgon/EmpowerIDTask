namespace BlogService.Features.Posts.Domain;

public class Comment
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public string CreatedBy { get; set; }
    public string Content { get; set; }
}