
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;

//var staticFileProvider = new StaticFileProvider(builder.Environment.WebRootPath);

builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	// ArrayModelBinderProvider 
	// CollectionModelBinderProvider 
	// ComplexObjectModelBinderProvider 

	//options.ModelBinderProviders.Insert(0, new JsonModelBinderProvider());
	//options.ValueProviderFactories.Add(new JsonValueProviderFactory(new JsonSerializerOptions()
	//{
	//	NumberHandling = JsonNumberHandling.AllowReadingFromString
	//}));

	options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory());

	//options.ValueProviderFactories.Add(new JsonParametersValueProviderFactory());

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
