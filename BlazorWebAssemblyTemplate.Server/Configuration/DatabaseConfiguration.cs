using Microsoft.Data.Sqlite;
using System.Data;

namespace BlazorWebAssemblyTemplate.Server.Configuration
{
    public static class DatabaseConfiguration
    {
        public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IDbConnection>(db =>
                new SqliteConnection(builder.Configuration.GetConnectionString("LocalConnection")));

            return builder;
        }
    }
}
