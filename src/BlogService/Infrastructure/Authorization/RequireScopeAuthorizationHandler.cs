using Microsoft.AspNetCore.Authorization;

namespace BlogService.Infrastructure.Authorization;

public class RequireScopeAuthorizationHandler : AuthorizationHandler<RequireScope>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireScope requirement)
    {
        var userScopes = context.User.FindAll("scope");

        if(userScopes.Any(s => s.Value.Equals(requirement.ScopeName)))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, $"User does not have required scope '{requirement.ScopeName}'."));
        }

        return Task.CompletedTask;
    }
}
