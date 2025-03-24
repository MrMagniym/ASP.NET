using WebApiApp.Models;

namespace WebApiApp.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(ApiCorsPolicies.AllowAllOrigins, policy =>
            {
                policy
                    .AllowAnyOrigin() 
                    .AllowAnyMethod() 
                    .AllowAnyHeader();
            });

            options.AddPolicy(ApiCorsPolicies.AllowSpecificOrigins, policy =>
            {
                policy.WithOrigins("http://localhost:5272")
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyHeader();
            });

            options.AddPolicy(ApiCorsPolicies.AllowMultipleOrigins, policy =>
            {
                policy.WithOrigins("https://example.com", "http://example.com", "http://localhost:5272")
                    .WithMethods("GET", "POST")
                    .AllowCredentials()
                    .WithHeaders("Origin", "Content-Type", "Authorization");
            });
        });

        return services;
    }
}
