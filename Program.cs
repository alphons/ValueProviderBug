
using Heijden.AspNetCore.Mvc.ModelBinding;


var builder = WebApplication.CreateBuilder();

builder.Services.AddMvcCoreCorrected();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.UseSession();
app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
