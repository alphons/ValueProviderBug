using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

#nullable enable

public class JsonGetModelProvider : IGetModelProvider, IValueProvider // IValueProvider for compatibility reasons
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	private readonly JsonDocument? jsonDocument;

	public JsonGetModelProvider(JsonDocument? jsonDocument, JsonSerializerOptions? options)
	{
		this.jsonSerializerOptions = options;

		this.jsonDocument = jsonDocument;
	}

	public bool ContainsPrefix(string prefix)
	{
		if (jsonDocument == null)
			return false;
		else
			return jsonDocument.RootElement.TryGetProperty(prefix, out _);
	}

	/// <summary>
	/// Returns object of type when available
	/// </summary>
	/// <param name="key">name of the model</param>
	/// <param name="t">type of the model</param>
	/// <returns>null or object model of type</returns>
	public object? GetModel(string key, Type t)
	{
		if (jsonDocument == null)
			return null;
		else
			return jsonDocument.RootElement.GetProperty(key).Deserialize(t, jsonSerializerOptions);
	}

	/// <summary>
	/// Obsolete, use GetModel instead
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	[Obsolete("Use GetModel")]
	public ValueProviderResult GetValue(string key)
	{
		return ValueProviderResult.None;
	}
}

public class JsonGetModelProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, JsonSerializerOptions? options)
	{
		try
		{
			var jsonDocument = await JsonDocument.ParseAsync(context.ActionContext.HttpContext.Request.Body);

			context.ValueProviders.Add(new JsonGetModelProvider(jsonDocument, options));
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
	public JsonGetModelProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public JsonGetModelProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


