using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using MyLittleLibrary.Components;
using MyLittleLibrary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<MangaRepository>(_ => {
    var username = Environment.GetEnvironmentVariable("MONGODB_USERNAME");
    var password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD");
    var connectionString = $"mongodb://{username}:{password}@mongodb:27017";
    var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE");
    return new MangaRepository(connectionString, databaseName);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();