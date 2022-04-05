using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

#nullable enable
public class RouteGetModelProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, JsonSerializerOptions? options)
	{
		try
		{
			await Task.Yield();

			var request = context.ActionContext.HttpContext.Request;
			var jsonString = JsonSerializer.Serialize(request.RouteValues);
			var jsonDocument = JsonDocument.Parse(jsonString, options: default);

			context.ValueProviders.Add(new GetModelProvider(jsonDocument, options));
		}
		catch (Exception eee)
		{
			// Not valid json, dont bother
			Debug.WriteLine(eee.Message);
		}
	}

	Task IValueProviderFactory.CreateValueProviderAsync(ValueProviderFactoryContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		return AddValueProviderAsync(context, this.jsonSerializerOptions);
	}
	public RouteGetModelProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public RouteGetModelProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


