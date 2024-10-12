using BlazorWebAssemblyTemplate.Shared.Models;

namespace BlazorWebAssemblyTemplate.Server.Configuration
{
    public static class PoliciesConfiguration
    {
        public static WebApplicationBuilder ConfigurePolicies(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("UserPolicy", policy =>
                   policy.RequireRole(UserRole.User.ToString(), UserRole.Admin.ToString()))
                .AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole(UserRole.Admin.ToString()));

            return builder;
        }
    }
}
