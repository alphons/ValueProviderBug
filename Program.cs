
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder();

var services = builder.Services;

//var staticFileProvider = new StaticFileProvider(builder.Environment.WebRootPath);

builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	//options.OutputFormatters.Clear();
	options.InputFormatters.Clear();
	options.ValueProviderFactories.Clear();
	options.ModelValidatorProviders.Clear();
	options.Conventions.Clear();
	options.Filters.Clear();
	options.ModelMetadataDetailsProviders.Clear();
	options.ModelValidatorProviders.Clear();
	options.ModelMetadataDetailsProviders.Clear();
	options.ModelBinderProviders.Clear();

	options.ModelBinderProviders.Add(new ValueProviderBug.SimpleTypeModelBinderProvider()); // "" -> null
	options.ModelBinderProviders.Add(new ValueProviderBug.CollectionModelBinderProvider()); // Problems having null values

	//options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory());
	options.ValueProviderFactories.Add(new CoreBasic.CorePlus.MiddleWare.JsonParametersValueProviderFactory());

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
