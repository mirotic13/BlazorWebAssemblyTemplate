using BlazorWebAssemblyTemplate.Server.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .ConfigureDatabase()
    .ConfigureRepositories()
    .ConfigureServices()
    .ConfigureCustomAuthentication()
    .ConfigurePolicies()
    .ConfigureCustomSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Forbidden)
    {
        response.ContentType = "application/json";
        var errorMessage = new { message = "No tienes permisos para acceder a este recurso." };
        await response.WriteAsJsonAsync(errorMessage);
    }
    else if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        response.ContentType = "application/json";
        var errorMessage = new { message = "Se necesita autenticación para acceder a este servidor" };
        await response.WriteAsJsonAsync(errorMessage);
    }
});

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
