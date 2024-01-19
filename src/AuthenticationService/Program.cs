using AuthenticationService;
using IdentityServer4.Hosting;
using IdentityServer4.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryApiScopes(new[] { new ApiScope("post.read"), new ApiScope("post.create"), new ApiScope("comment.create") })
    .AddInMemoryApiResources(new[] { new ApiResource
    {
        Name = "PostsApi",
        Scopes = new[] { "post.read", "post.create", "comment.create" }
    } })
    .AddInMemoryClients(new[] { new Client
    {
        AllowedScopes = new[] { "post.read", "post.create", "comment.create" },
        AllowedGrantTypes = new[] { GrantType.ClientCredentials },
        ClientId = "client",
        ClientSecrets = new[] { new Secret("secret".Sha256()) }
    }});

var app = builder.Build();

app.UseMiddleware<IdentityServerUrlMiddleware>(app.Configuration.GetValue<string>("Issuer"));
app.ConfigureCors();
app.UseMiddleware<IdentityServerMiddleware>();

app.Run();
