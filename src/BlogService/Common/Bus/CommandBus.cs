using BlogService.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlogService.Common.Bus;

public sealed class CommandBus(IServiceProvider serviceProvider) : ICommandBus
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();

        return handler.Handle(command, cancellation);
    }
}
