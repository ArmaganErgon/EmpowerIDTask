using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Model;
using BlogService.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using System.Net.Mime;

namespace BlogService.Features.Posts.Apis
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("PerClient")]
    public class PostsController(IQueryBus queryBus, ICommandBus commandBus) : ControllerBase
    {
        private readonly IQueryBus queryBus = queryBus ?? throw new ArgumentNullException(nameof(queryBus));
        private readonly ICommandBus commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));

        [HttpGet("{id:long}")]
        [RequireScope("post.read")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(GetPostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> Get([BindRequired] long id,  CancellationToken ct)
        {
            var post = await queryBus.Dispatch<GetPostRequest, GetPostResponse?>(new(id), ct);
            if(post is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(post);
        }

        [HttpPost]
        [RequireScope("post.create")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatePostResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> Create([FromBody] CreatePostRequest request, CancellationToken ct)
        {
            var createResult = await commandBus.Dispatch<CreatePostRequest, CreatePostResponse>(request, ct);
            return TypedResults.Created(Url.Action(nameof(Get), new { id = createResult.Id }), createResult);
        }

        [HttpPost]
        [Route("Comments")]
        [RequireScope("comment.create")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreateCommentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> CreateComment([FromBody] CreateCommentRequest request, CancellationToken ct)
        {
            var createResult = await commandBus.Dispatch<CreateCommentRequest, CreateCommentResponse>(request, ct);
            return TypedResults.Created(Url.Action(nameof(Get), new { id = createResult.Id }), createResult);
        }
    }
}
