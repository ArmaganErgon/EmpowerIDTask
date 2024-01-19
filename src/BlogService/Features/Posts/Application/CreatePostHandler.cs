using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Model;
using FluentValidation;
using IdGen;
using Riok.Mapperly.Abstractions;

namespace BlogService.Features.Posts.Application;

public class CreatePostHandler(IValidator<CreatePostRequest> validator, IRepository<Post> repository, IdGenerator idGenerator) 
    : ICommandHandler<CreatePostRequest, CreatePostResponse>
{
    private readonly IValidator<CreatePostRequest> validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IRepository<Post> repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IdGenerator idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

    public async Task<CreatePostResponse> Handle(CreatePostRequest command, CancellationToken ct = default)
    {
        await Validate(command, ct);

        var entity = command.ToPost();
        entity.Id = idGenerator.CreateId();

        await repository.AddAsync(entity, ct);

        return new(entity.Id);
    }

    private async Task Validate(CreatePostRequest command, CancellationToken ct = default)
    {
        var validationResults = await validator.ValidateAsync(command, ct);
        if (!validationResults.IsValid)
        {
            throw new ValidationException(validationResults.Errors);
        }
    }
}

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(c => c.CreatedBy).NotEmpty();
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Content).NotEmpty();
    }
}

[Mapper]
public static partial class CreatePostRequestMapper
{
    public static partial Post ToPost(this CreatePostRequest input);
}