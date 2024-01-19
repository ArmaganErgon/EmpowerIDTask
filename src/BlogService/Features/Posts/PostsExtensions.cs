using BlogService.Common.Interfaces;
using BlogService.Features.Comments.Application;
using BlogService.Features.Posts.Application;
using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Infrastructure.Persistence;
using BlogService.Features.Posts.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Features.Posts;

public static class PostsExtensions
{
    public static IServiceCollection AddPosts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IValidator<GetPostRequest>, GetPostRequestValidator>();
        services.AddScoped<IValidator<CreatePostRequest>, CreatePostRequestValidator>();
        services.AddScoped<IValidator<CreateCommentRequest>, CreateCommentRequestValidator>();

        services.AddDbContext<PostsDbContext>(config =>
        {
            config.UseSqlServer(configuration.GetConnectionString("PostsDB"));
        });

        services.AddScoped<IRepository<Post>, PostsRepository>();
        services.AddScoped<IRepository<Comment>, CommentsRepository>();

        services.AddScoped<IQueryHandler<GetPostRequest, GetPostResponse>, GetPostHandler>();
        services.AddScoped<ICommandHandler<CreatePostRequest, CreatePostResponse>, CreatePostHandler>();
        services.AddScoped<ICommandHandler<CreateCommentRequest, CreateCommentResponse>, CreateCommentHandler>();

        return services;
    }
}
