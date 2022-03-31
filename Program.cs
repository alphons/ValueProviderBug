
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var ModelBinderProviders = new string[] {
		"BinderTypeModelBinderProvider",
		"ServicesModelBinderProvider",
		"BodyModelBinderProvider",
		"HeaderModelBinderProvider",
		"FloatingPointTypeModelBinderProvider",
		"EnumTypeModelBinderProvider",
		"DateTimeModelBinderProvider",
		"CancellationTokenModelBinderProvider",
		"ByteArrayModelBinderProvider",
		"FormFileModelBinderProvider",
		"FormCollectionModelBinderProvider",
		"KeyValuePairModelBinderProvider",
		"DictionaryModelBinderProvider",
		"ComplexObjectModelBinderProvider",
		"ArrayModelBinderProvider",

		// "CollectionModelBinderProvider", // Dont remove for this test
		// "SimpleTypeModelBinderProvider", // Dont remove for this test
	};

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

	// Removing binderproviders not used in this test
	foreach (var binder in options.ModelBinderProviders.ToArray())
	{
		var name = binder.GetType().Name;
		if (ModelBinderProviders.Contains(name))
		{
			Debug.WriteLine($"Removing \"{name}\"");
			options.ModelBinderProviders.Remove(binder);
		}
	}

	options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory());
});

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.Run();
