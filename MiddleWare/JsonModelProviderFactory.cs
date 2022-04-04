using System.Diagnostics;

// JsonValueProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

#nullable enable

public interface IModelProvider
{
	object? GetModel(string key, Type t);
	bool ContainsPrefix(string prefix);
}


public class JsonModelProvider : IModelProvider, IValueProvider // IValueProvider for compatibility reasons
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	private readonly JsonDocument? jsonDocument;

	public JsonModelProvider(JsonDocument? jsonDocument, JsonSerializerOptions? options)
	{
		this.jsonSerializerOptions = options;

		this.jsonDocument = jsonDocument;
	}

	public bool ContainsPrefix(string prefix)
	{
		Debug.WriteLine($"Prefix:{prefix}");
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
		if (jsonDocument == null)
			return ValueProviderResult.None;

		if (jsonDocument.RootElement.TryGetProperty(key, out JsonElement el) == false)
			return ValueProviderResult.None;
		
		switch (el.ValueKind)
		{
			case JsonValueKind.String:
			case JsonValueKind.Number:
				return new ValueProviderResult(new string[] { el.ToString() });
			case JsonValueKind.True:
				return new ValueProviderResult(new string[] { "true" });
			case JsonValueKind.False:
				return new ValueProviderResult(new string[] { "false" });
			case JsonValueKind.Array:
				return ValueProviderResult.None;
			case JsonValueKind.Null:
				return new ValueProviderResult(new string[] { null });
			default:
				return ValueProviderResult.None;
		}
	}
}

public class JsonModelProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, JsonSerializerOptions? options)
	{
		try
		{
			var jsonDocument = await JsonDocument.ParseAsync(context.ActionContext.HttpContext.Request.Body);

			context.ValueProviders.Add(new JsonModelProvider(jsonDocument, options));
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
	public JsonModelProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public JsonModelProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


