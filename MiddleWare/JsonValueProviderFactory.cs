using System.Diagnostics;

// JsonValueProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

#nullable enable

public class JsonValueProvider : IValueProvider
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	private readonly JsonDocument? doc;

	public JsonValueProvider(JsonDocument? doc, JsonSerializerOptions? options)
	{
		this.jsonSerializerOptions = options;

		this.doc = doc;
	}

	public bool ContainsPrefix(string prefix)
	{
		if (doc == null)
			return false;
		else
			return doc.RootElement.TryGetProperty(prefix, out _);
	}

	public object? GetModel(string key, Type t)
	{
		if (doc == null)
			return null;
		else
			return doc.RootElement.GetProperty(key).Deserialize(t, jsonSerializerOptions);
	}

	/// <summary>
	/// Obsolete, use GetObject instead
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public ValueProviderResult GetValue(string key)
	{
		return ValueProviderResult.None;
	}
}

public class JsonValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, JsonSerializerOptions? options)
	{
		try
		{
			var jsonDocument = await JsonDocument.ParseAsync(context.ActionContext.HttpContext.Request.Body);

			context.ValueProviders.Add(new JsonValueProvider(jsonDocument, options));
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

		var Request = context.ActionContext.HttpContext.Request;
		if (Request.Method == "POST")
		{
			if (Request.ContentType == null || Request.ContentType.StartsWith("application/json"))
			{
				if (Request.ContentLength == null || // Chunked encoding
					Request.ContentLength >= 2) // Normal encoding, using content length minimum '{}'
					return AddValueProviderAsync(context, this.jsonSerializerOptions);
			}
		}
		return Task.CompletedTask;
	}
	public JsonValueProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public JsonValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


