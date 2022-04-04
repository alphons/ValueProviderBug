using System.Diagnostics;

// (C) 2022 Alphons van der Heijden
// Date: 2022-03-30
// Version: 2.2n

using System.Text.Json;

using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding;

#nullable enable

namespace ValueProviderBug;

public interface IJsonGetOject
{
	object? GetObject(string key, Type t);
}

public class JsonValueProvider : IValueProvider, IJsonGetOject
{
	private readonly CultureInfo CultureInfo;
	private readonly JsonDocument? doc;

	public JsonValueProvider(JsonDocument? doc, CultureInfo CultureInfo)
	{
		this.CultureInfo = CultureInfo;

		this.doc = doc;
	}

	public bool ContainsPrefix(string prefix)
	{
		if (doc == null)
			return false;
		else
			return doc.RootElement.TryGetProperty(prefix, out _);
	}

	public object? GetObject(string key, Type t)
	{
		if (doc == null)
			return null;
		else
			return doc.RootElement.GetProperty(key).Deserialize(t);
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
	private readonly CultureInfo CultureInfo;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, CultureInfo CultureInfo)
	{
		try
		{
			var jsonDocument = await JsonDocument.ParseAsync(context.ActionContext.HttpContext.Request.Body);

			context.ValueProviders.Add(new JsonValueProvider(jsonDocument, CultureInfo));
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
					return AddValueProviderAsync(context, this.CultureInfo);
			}
		}
		return Task.CompletedTask;
	}
	public JsonValueProviderFactory(string CultureName) : base()
	{
		this.CultureInfo = new CultureInfo(CultureName);
	}

	public JsonValueProviderFactory() : base()
	{
		this.CultureInfo = CultureInfo.InvariantCulture;
	}
}


