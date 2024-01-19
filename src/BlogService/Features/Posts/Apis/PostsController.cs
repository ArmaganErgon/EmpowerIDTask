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
        public async Task<IActionResult> Get([BindRequired] long id)
        {
            return new JsonResult(await queryBus.Dispatch<GetPostRequest, GetPostResponse>(new(id)));
        }

        [HttpPost]
        [RequireScope("post.create")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatePostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            return new JsonResult(await commandBus.Dispatch<CreatePostRequest, CreatePostResponse>(request));
        }

        [HttpPost]
        [Route("Comments")]
        [RequireScope("comment.create")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreatePostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            return new JsonResult(await commandBus.Dispatch<CreateCommentRequest, CreateCommentResponse>(request));
        }
    }
}
