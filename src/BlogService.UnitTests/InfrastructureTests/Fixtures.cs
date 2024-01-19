using BlogService.Features.Posts.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BlogService.UnitTests.InfrastructureTests;

public class PostsDbContextFixture : IDisposable
{
    public PostsDbContext PostsDbContext { get; private set; }
    private SqliteConnection? connection;

    public PostsDbContextFixture()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<PostsDbContext>()
                .UseSqlite(connection)
                .Options;

        PostsDbContext = new PostsDbContext(contextOptions);

        Assert.True(PostsDbContext.Database.EnsureCreated());
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Dispose();
                connection = null;
            }
        }
    }
}