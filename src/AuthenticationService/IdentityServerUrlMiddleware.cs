using IdentityServer4.Extensions;

namespace AuthenticationService;

// To override IdentityServer's base url with Docker network name.
public class IdentityServerUrlMiddleware
{
    private readonly RequestDelegate next;
    private readonly string publicFacingUri;

    public IdentityServerUrlMiddleware(RequestDelegate next, string publicFacingUri)
    {
        this.publicFacingUri = publicFacingUri;
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        context.SetIdentityServerOrigin(publicFacingUri);
        context.SetIdentityServerBasePath(request.PathBase.Value.TrimEnd('/'));

        await next(context);
    }
}
