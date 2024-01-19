namespace BlogService.Common.Interfaces;

public interface ICommandBus
{
    Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken ct = default);
}