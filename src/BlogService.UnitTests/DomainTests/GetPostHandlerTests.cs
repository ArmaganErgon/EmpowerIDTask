using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Application;
using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Infrastructure.Persistence;
using BlogService.Features.Posts.Model;
using BlogService.UnitTests.InfrastructureTests;
using FluentValidation;
using Moq;

namespace BlogService.UnitTests.DomainTests;

public class GetPostHandlerTests
{
    private static IValidator<GetPostRequest> Validator => new GetPostRequestValidator();

    [Fact]
    public async Task GetPost_Should_Succeed()
    {
        using var fixture = new PostsDbContextFixture();
        var mockRepository = new PostsRepository(fixture.PostsDbContext);
        var mockCacheProvider = new Mock<ICacheProvider>();
        mockCacheProvider.Setup(c => c.GetAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((string?)null);

        long postId = 1;
        var addedEntryCount = await mockRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Title = "test", Content = "test" });

        Assert.Equal(1, addedEntryCount);

        var handler = new GetPostHandler(Validator, mockRepository, mockCacheProvider.Object);

        var result = await handler.Handle(new GetPostRequest { Id = postId });

        Assert.NotNull(result.Post);
        Assert.Equal(postId, result.Post.Id);
    }

    [Fact]
    public async Task GetPost_Should_Fail_Validation()
    {
        using var fixture = new PostsDbContextFixture();
        var mockRepository = new PostsRepository(fixture.PostsDbContext);
        var mockCacheProvider = new Mock<ICacheProvider>();
        mockCacheProvider.Setup(c => c.GetAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((string?)null);

        var handler = new GetPostHandler(Validator, mockRepository, mockCacheProvider.Object);

        await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(new GetPostRequest { Id = default }));
    }
}
