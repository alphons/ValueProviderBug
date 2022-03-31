
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	options.OutputFormatters.Clear();
	options.InputFormatters.Clear();
	options.ValueProviderFactories.Clear();
	options.ModelValidatorProviders.Clear();
	options.Conventions.Clear();
	options.Filters.Clear();
	options.ModelMetadataDetailsProviders.Clear();
	options.ModelValidatorProviders.Clear();
	options.ModelMetadataDetailsProviders.Clear();
	//options.ModelBinderProviders.Clear();

	options.ModelBinderProviders.Add(new TestWeb.SimpleTypeNoNullModelBinderProvider()); // "" -> ""
	options.ModelBinderProviders.Add(new CollectionModelBinderProvider()); // Problems having null values

	
	//options.ModelBinderProviders.Add(new SimpleTypeModelBinderProvider()); // "" -> null

	//options.ModelBinderProviders.Add(new TestWeb.MyCollectionModelBinderProvider());

	options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory());

});

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.Run();
