using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogService.UnitTests.InfrastructureTests
{
    public class PostsRepositoryTests()
    {
        [Fact]
        public async Task GetPost_Should_Succeed()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);

            var post = await mockPostsRepository.GetByIdAsync(1);

            Assert.Null(post);

            await mockPostsRepository.AddAsync(new Post { Id = 1, CreatedBy = "test", Title = "test", Content = "test" });

            post = await mockPostsRepository.GetByIdAsync(1);

            Assert.NotNull(post);
            Assert.Equal(1, post.Id);
        }

        [Fact]
        public async Task GetPost_Should_Return_Null()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);

            var post = await mockPostsRepository.GetByIdAsync(2);

            Assert.Null(post);
        }

        [Fact]
        public async Task AddPost_Should_Succeed()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);

            var addedEntryCount = await mockPostsRepository.AddAsync(new Post { Id = 3, CreatedBy = "test", Title = "test", Content = "test" });

            Assert.Equal(1, addedEntryCount);
        }

        [Fact]
        public async Task AddPost_Without_CratedBy_Should_Fail()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await mockPostsRepository.AddAsync(new Post { Id = 3, Title = "test", Content = "test" }));
        }

        [Fact]
        public async Task AddPost_Without_Title_Should_Fail()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await mockPostsRepository.AddAsync(new Post { Id = 3, CreatedBy = "test", Content = "test" }));
        }

        [Fact]
        public async Task AddPost_Without_Content_Should_Fail()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await mockPostsRepository.AddAsync(new Post { Id = 3, CreatedBy = "test", Title = "test" }));
        }
    }
}