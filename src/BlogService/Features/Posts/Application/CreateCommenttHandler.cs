using BlogService.Common.Interfaces;
using BlogService.Features.Posts.Domain;
using BlogService.Features.Posts.Model;
using FluentValidation;
using IdGen;
using Riok.Mapperly.Abstractions;

namespace BlogService.Features.Comments.Application;

public class CreateCommentHandler(IValidator<CreateCommentRequest> validator, IRepository<Comment> repository, IdGenerator idGenerator)
    : ICommandHandler<CreateCommentRequest, CreateCommentResponse>
{
    private readonly IValidator<CreateCommentRequest> validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IRepository<Comment> repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IdGenerator idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

    public async Task<CreateCommentResponse> Handle(CreateCommentRequest command, CancellationToken ct = default)
    {
        await Validate(command, ct);

        var entity = command.ToComment();
        entity.Id = idGenerator.CreateId();

        await repository.AddAsync(entity, ct);

        return new(entity.Id);
    }

    private async Task Validate(CreateCommentRequest command, CancellationToken ct = default)
    {
        var validationResults = await validator.ValidateAsync(command, ct);
        if (!validationResults.IsValid)
        {
            throw new ValidationException(validationResults.Errors);
        }
    }
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(c => c.PostId).NotEmpty();
        RuleFor(c => c.CreatedBy).NotEmpty();
        RuleFor(c => c.Content).NotEmpty();
    }
}

[Mapper]
public static partial class CreateCommentRequestMapper
{
    public static partial Comment ToComment(this CreateCommentRequest input);
}