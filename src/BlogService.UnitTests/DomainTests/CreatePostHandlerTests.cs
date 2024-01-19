using BlogService.Features.Posts.Application;
using BlogService.Features.Posts.Infrastructure.Persistence;
using BlogService.Features.Posts.Model;
using BlogService.UnitTests.InfrastructureTests;
using FluentValidation;
using IdGen;

namespace BlogService.Unitests.DomainTests;

public class CreatePostHandlerTests
{
    private static IValidator<CreatePostRequest> Validator => new CreatePostRequestValidator();

    private static IdGenerator IdGenerator => new(Random.Shared.Next(1, 1024));

    [Fact]
    public async Task CreatePost_Should_Succeed()
    {
        using var fixture = new PostsDbContextFixture();
        var mockRepository = new PostsRepository(fixture.PostsDbContext);

        var handler = new CreatePostHandler(Validator, mockRepository, IdGenerator);

        var result = await handler.Handle(new CreatePostRequest { CreatedBy = "test", Title = "test", Content = "test" });

        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreatePost_Empty_CreatedBy_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockRepository = new PostsRepository(fixture.PostsDbContext);

        var handler = new CreatePostHandler(Validator, mockRepository, IdGenerator);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new CreatePostRequest { Content = "test", Title = "test" }));
    }

    [Fact]
    public async Task CreatePost_Empty_Content_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockRepository = new PostsRepository(fixture.PostsDbContext);

        var handler = new CreatePostHandler(Validator, mockRepository, IdGenerator);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new CreatePostRequest { CreatedBy = "test", Title = "test" }));
    }

    [Fact]
    public async Task CreatePost_Empty_Title_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockRepository = new PostsRepository(fixture.PostsDbContext);

        var handler = new CreatePostHandler(Validator, mockRepository, IdGenerator);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new CreatePostRequest { Content = "test", CreatedBy = "test" }));
    }
}
