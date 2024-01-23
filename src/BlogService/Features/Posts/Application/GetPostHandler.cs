using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Model;
using FluentValidation;
using Riok.Mapperly.Abstractions;
using System.Text.Json;

namespace BlogService.Features.Posts.Application;

public class GetPostHandler(IValidator<GetPostRequest> validator, IRepository<Post> repository, ICacheProvider cacheProvider)
    : IQueryHandler<GetPostRequest, GetPostResponse?>
{
    private readonly IValidator<GetPostRequest> validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IRepository<Post> repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ICacheProvider cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
    private const string cacheKeyPrefix = "Post";

    public async Task<GetPostResponse?> Handle(GetPostRequest query, CancellationToken ct = default)
    {
        await Validate(query, ct);

        string cacheKey = $"{cacheKeyPrefix}_{query.Id}";

        var cached = await cacheProvider.GetAsync(cacheKey, ct);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<GetPostResponse>(cached);
        }

        var entity = await repository.GetByIdAsync(query.Id, ct);
        var response = entity?.ToResponse();

        if(response is not null)
            await cacheProvider.AddAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(5), ct);

        return response;
    }

    private async Task Validate(GetPostRequest query, CancellationToken ct = default)
    {
        var validationResults = await validator.ValidateAsync(query, ct);
        if (!validationResults.IsValid)
        {
            throw new ValidationException(validationResults.Errors);
        }
    }
}

public class GetPostRequestValidator : AbstractValidator<GetPostRequest>
{
    public GetPostRequestValidator()
    {
        RuleFor(q => q.Id)
            .NotNull()
            .NotEqual(x => default);
    }
}

[Mapper]
public static partial class GetPostResponseMapper
{
    public static partial GetPostResponse ToResponse(this Post input);
}