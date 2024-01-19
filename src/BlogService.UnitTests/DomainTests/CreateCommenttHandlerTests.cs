using BlogService.Features.Comments.Application;
using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Infrastructure.Persistence;
using BlogService.Features.Posts.Model;
using BlogService.UnitTests.InfrastructureTests;
using FluentValidation;
using IdGen;

namespace BlogService.Unitests.DomainTests;

public class CreateCommentHandlerTests
{
    private static IValidator<CreateCommentRequest> Validator => new CreateCommentRequestValidator();

    private static IdGenerator IdGenerator => new(Random.Shared.Next(1, 1024));

    [Fact]
    public async Task CreateComment_Should_Succeed()
    {
        using var fixture = new PostsDbContextFixture();
        var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);
        var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

        long postId = IdGenerator.CreateId();
        var writtenEntryCount = await mockPostsRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Title = "test", Content = "test" });

        Assert.Equal(1, writtenEntryCount);

        var handler = new CreateCommentHandler(Validator, mockCommentsRepository, IdGenerator);

        var result = await handler.Handle(new CreateCommentRequest { PostId = postId, CreatedBy = "test", Content = "test" });

        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateComment_Empty_CreatedBy_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

        var handler = new CreateCommentHandler(Validator, mockCommentsRepository, IdGenerator);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new CreateCommentRequest { PostId = 1, Content = "test" }));
    }

    [Fact]
    public async Task CreateComment_Empty_Content_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);
        var idGenerator = IdGenerator;

        var handler = new CreateCommentHandler(Validator, mockCommentsRepository, IdGenerator);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new CreateCommentRequest { PostId = 1, CreatedBy = "test" }));
    }

    [Fact]
    public async Task CreateComment_Empty_PostId_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

        var handler = new CreateCommentHandler(Validator, mockCommentsRepository, IdGenerator);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new CreateCommentRequest { CreatedBy = "test", Content = "test" }));
    }
}
