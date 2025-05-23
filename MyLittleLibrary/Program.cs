using MudBlazor.Services;
using MyLittleLibrary.Components;
using MyLittleLibrary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<MangaRepository>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetSection("MongoDB:ConnectionString").Value;
    var databaseName = config.GetSection("MongoDB:DatabaseName").Value;
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