using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Comment = BlogService.Features.Posts.Domain.Comment;

namespace BlogService.UnitTests.InfrastructureTests
{
    public class CommentsRepositoryTests()
    {
        [Fact]
        public async Task GetComment_Should_Succeed()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);
            var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

            long postId = 1;
            var addedPostEntryCount = await mockPostsRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Content = "test", Title = "test" });

            Assert.Equal(1, addedPostEntryCount);

            var addedEntryCount = await mockCommentsRepository.AddAsync(new Comment { Id = 1, PostId = postId, CreatedBy = "test", Content = "test" });
            Assert.Equal(1, addedEntryCount);

            var comment = await mockCommentsRepository.GetByIdAsync(1);

            Assert.NotNull(comment);
            Assert.Equal(1, comment.Id);
        }

        [Fact]
        public async Task GetComment_Should_Return_Null()
        {
            using var fixture = new PostsDbContextFixture();
            var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

            var Comment = await mockCommentsRepository.GetByIdAsync(1);

            Assert.Null(Comment);
        }

        [Fact]
        public async Task AddComment_Should_Succeed()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);
            var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

            long postId = 1;
            var addedPostEntryCount = await mockPostsRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Content = "test", Title = "test" });

            Assert.Equal(1, addedPostEntryCount);

            var addedEntryCount = await mockCommentsRepository.AddAsync(new Comment { Id = 1, PostId = postId, CreatedBy = "test", Content = "test" });

            Assert.Equal(1, addedEntryCount);
        }

        [Fact]
        public async Task AddComment_Without_CratedBy_Should_Fail()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);
            var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

            long postId = 1;
            var addedEntryCount = await mockPostsRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Content = "test", Title = "test" });

            Assert.Equal(1, addedEntryCount);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await mockCommentsRepository.AddAsync(new Comment { Id = 1, PostId = postId, Content = "test" }));
        }

        [Fact]
        public async Task AddComment_Without_Content_Should_Fail()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);
            var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

            long postId = 1;
            var addedEntryCount = await mockPostsRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Content = "test", Title = "test" });

            Assert.Equal(1, addedEntryCount);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await mockCommentsRepository.AddAsync(new Comment { Id = 1, PostId = postId, CreatedBy = "test" }));
        }

        [Fact]
        public async Task AddComment_Without_PostId_Should_Fail()
        {
            using var fixture = new PostsDbContextFixture();
            var mockPostsRepository = new PostsRepository(fixture.PostsDbContext);
            var mockCommentsRepository = new CommentsRepository(fixture.PostsDbContext);

            long postId = 1;
            var addedEntryCount = await mockPostsRepository.AddAsync(new Post { Id = postId, CreatedBy = "test", Content = "test", Title = "test" });

            Assert.Equal(1, addedEntryCount);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await mockCommentsRepository.AddAsync(new Comment { Id = 1, CreatedBy = "test", Content = "test" }));
        }
    }
}