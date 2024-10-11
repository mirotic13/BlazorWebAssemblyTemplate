using BlazorWebAssemblyTemplate.Server.Services;

namespace BlazorWebAssemblyTemplate.Server.Configuration
{
    public static class ServicesConfiguration
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<IUserService, UserService>();

            return builder;
        }
    }
}
