using LinkPulseDefinitions;
using LinkPulseImplementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHashProvider, SHA256HashProvider>();
builder.Services.AddSingleton<IStorage, RedisStorage>();
builder.Services.AddSingleton<IShortenerController, ShortenerController>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
app.MapControllerRoute(
    name: "shortened",
    pattern: "{param?}",
    defaults: new { Controller = "Home", Action = "Index" });
app.MapControllers();

app.Run();
