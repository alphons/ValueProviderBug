using System.Collections;
using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

#nullable enable

public class GetModelProvider : BindingGetModelProvider
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	private readonly JsonDocument? jsonDocument;

	public GetModelProvider(BindingSource bindingSource, JsonDocument? jsonDocument, JsonSerializerOptions? options) : base(bindingSource)
	{
		this.jsonSerializerOptions = options;

		this.jsonDocument = jsonDocument;
	}

	public override bool ContainsPrefix(string prefix)
	{
		if (jsonDocument == null)
			return false;

		if (jsonDocument.RootElement.ValueKind == JsonValueKind.Object)
			return jsonDocument.RootElement.TryGetProperty(prefix, out _);
		else
			return false;
	}

	/// <summary>
	/// Returns object of type when available
	/// </summary>
	/// <param name="key">name of the model</param>
	/// <param name="t">type of the model</param>
	/// <returns>null or object model of type</returns>
	public object? GetModel(string key, Type t)
	{
		if (jsonDocument != null)
		{
			var prop = jsonDocument.RootElement.GetProperty(key);
			// this needs some tweaking!!!
			if (prop.ValueKind != JsonValueKind.Array || t.IsArray || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)))
			{
				return prop.Deserialize(t, jsonSerializerOptions);
			}
			else
			{
				var first = prop.EnumerateArray().FirstOrDefault();
				if (first.ValueKind != JsonValueKind.Null)
					return first.Deserialize(t, jsonSerializerOptions);
			}
		}
		return null;
	}

}


