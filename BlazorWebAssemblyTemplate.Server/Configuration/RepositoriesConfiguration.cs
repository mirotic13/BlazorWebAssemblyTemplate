using BlazorWebAssemblyTemplate.Server.Repositories;

namespace BlazorWebAssemblyTemplate.Server.Configuration
{
    public static class RepositoriesConfiguration
    {
        public static WebApplicationBuilder ConfigureRepositories(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<IUserRepository, UserRepository>();

            return builder;
        }
    }
}
