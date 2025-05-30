using MudBlazor.Services;
using MyLittleLibrary.Components;
using MyLittleLibrary.Infrastructure;
using MyLittleLibrary.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var username = Environment.GetEnvironmentVariable("MONGODB_USERNAME");
var password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD");
var connectionString = $"mongodb://{username}:{password}@mongodb:27017";
var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE");

builder.Services.Configure<MongoOptions>(options =>
{
    options.ConnectionString = connectionString;
    options.DatabaseName = databaseName;
});

builder.Services.AddOptions<MongoOptions>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<MangaRepository>()
    .AddClasses(classes => classes.InExactNamespaceOf<MangaRepository>())
    .AsSelf()
    .WithSingletonLifetime());

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