
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;

builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	options.ModelBinderProviders.Insert(0, new GenericModelBinderProvider());
	options.ValueProviderFactories.Add(new JsonModelProviderFactory(new JsonSerializerOptions()
	{
		NumberHandling = JsonNumberHandling.AllowReadingFromString
	}));
}).AddJsonOptions(options =>
{
	options.JsonSerializerOptions.DictionaryKeyPolicy = null;
	options.JsonSerializerOptions.PropertyNamingPolicy = null;					
});

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
