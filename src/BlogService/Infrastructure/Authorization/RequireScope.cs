using Microsoft.AspNetCore.Authorization;

namespace BlogService.Infrastructure.Authorization;

public class RequireScope(string scopeName) : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public string ScopeName { get; } = scopeName;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
