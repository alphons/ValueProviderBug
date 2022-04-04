
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

using ValueProviderBug;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;

//var staticFileProvider = new StaticFileProvider(builder.Environment.WebRootPath);

builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	//options.OutputFormatters.Clear();
	//options.InputFormatters.Clear();
	//options.ModelValidatorProviders.Clear();
	//options.Conventions.Clear();
	//options.Filters.Clear();
	//options.ModelMetadataDetailsProviders.Clear();
	//options.ModelValidatorProviders.Clear();
	//options.ModelMetadataDetailsProviders.Clear();

	options.ModelBinderProviders.Clear();
	options.ModelBinderProviders.Add(new JsonModelBinderProvider());

	options.ValueProviderFactories.Clear();
	options.ValueProviderFactories.Add(new JsonValueProviderFactory());


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
