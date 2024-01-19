using BlogService.Common.Bus;
using BlogService.Common.Interfaces;
using BlogService.Common.Providers;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace BlogService.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(context.ModelState)
                    {
                        ContentTypes = { Application.Json }
                    };
            });

        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryBus, QueryBus>();

        services.AddStackExchangeRedisCache(config =>
        {
            config.Configuration = configuration.GetValue<string>("Redis:ConnectionString");
        });

        services.AddSingleton<ICacheProvider, CacheProvider>();

        services.AddIdGen(Random.Shared.Next(1, 1024));

        return services;
    }
}
