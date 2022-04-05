
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Formatters;

namespace Alternative.DependencyInjection;

public static class MvcCoreCorrectedExtensions
{
	/// <summary>
	/// Cleanup the MVC pipeline
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	/// <returns>IMvcCoreBuilder</returns>
	public static IMvcCoreBuilder AddMvcCoreCorrected(this IServiceCollection services, bool CorrectDateTime = true)
	{
		return services.AddMvcCore().AddMvcOptions(options =>
		{
			options.InputFormatters.Clear();
			options.ValueProviderFactories.Clear();
			options.ModelValidatorProviders.Clear();
			options.Conventions.Clear();
			options.Filters.Clear();
			options.ModelMetadataDetailsProviders.Clear();
			options.ModelValidatorProviders.Clear();
			options.ModelMetadataDetailsProviders.Clear();
			options.ModelBinderProviders.Clear();
			options.OutputFormatters.Clear();

			var jsonSerializerOptions = new JsonSerializerOptions()
			{
				DictionaryKeyPolicy = null,
				PropertyNamingPolicy = null
			};

			if(CorrectDateTime)
				jsonSerializerOptions.Converters.Add(new DateTimeConverter());

			options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(jsonSerializerOptions));
			options.ModelBinderProviders.Insert(0, new GenericModelBinderProvider());
			options.ValueProviderFactories.Add(new JsonGetModelProviderFactory(new JsonSerializerOptions()
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString
			}));
		});
	}

	private class DateTimeConverter : JsonConverter<DateTime>
	{
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return DateTime.Parse(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToLocalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
		}
	}
}
