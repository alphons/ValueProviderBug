using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

#nullable enable
public class HeaderGetModelProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, JsonSerializerOptions? options)
	{
		try
		{
			await Task.Yield();

			var request = context.ActionContext.HttpContext.Request;
			var json = JsonSerializer.Serialize(request.Headers);

			//var list = request.Headers.Select(x => $"\"{x.Key}\": \"{x.Value[0]}\"").ToArray();
			//var json = $"{{{string.Join(',', list)}}}";

			var jsonDocument = JsonDocument.Parse(json, options: default);

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
	public HeaderGetModelProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public HeaderGetModelProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


