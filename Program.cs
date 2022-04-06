
using Heijden.AspNetCore.Mvc.ModelBinding;

var builder = WebApplication.CreateBuilder();

builder.Services.AddMvcCoreCorrected();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
