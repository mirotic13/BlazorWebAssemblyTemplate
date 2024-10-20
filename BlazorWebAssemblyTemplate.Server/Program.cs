using BlazorWebAssemblyTemplate.Server.Auth;
using BlazorWebAssemblyTemplate.Server.Configuration;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var expireMinutes = builder.Configuration.GetValue<int>("Jwt:ExpireMinutes");

builder.Services.AddHttpContextAccessor();

builder
    .ConfigureDatabase()
    .ConfigureRepositories()
    .ConfigureServices()
    .ConfigureCustomAuthentication()
    .ConfigurePolicies()
    .ConfigureCustomSwagger();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(expireMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseSession();

app.UseMiddleware<JwtSessionTokenMiddleware>();

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
