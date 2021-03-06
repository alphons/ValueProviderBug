using System.Diagnostics;

// (C) 2022 Alphons van der Heijden
// Date: 2022-03-30
// Version: 2.2n

//
// This works with the classic approach of the valueprovider/binder of ASP .NET Core
// However, there is a fundamental bug in the deserialize of the project using only key names
// When using Arrays having null elements, the system is broken
// Therefore we have to use he combination of these two:
// - JsonValueProviderFactory.cs
// - JsonModelBinderProvider.cs
//

using System.Text.Json;
using System.Text.Json.Nodes;

using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Heijden.AspNetCore.Mvc.ModelBinding;

public class JsonParametersValueProvider : IValueProvider
{
	private readonly CultureInfo CultureInfo;
	private readonly int MaxDepth = 100;
	private readonly Dictionary<string, ValueProviderResult> dict = new();

#nullable enable

	public JsonParametersValueProvider(JsonNode? jsonNode, CultureInfo CultureInfo)
	{
		this.CultureInfo = CultureInfo;

		BuildDictionary(0, jsonNode);
	}

	private void BuildDictionary(int Depth, JsonNode? jsonNode)
	{
		//Debug.WriteLine($"**{Depth}");
		if (jsonNode == null || Depth >= MaxDepth)
			return;

		Depth++;

		if (jsonNode is JsonObject)
		{
			dict.Add(jsonNode.GetPath(), ValueProviderResult.None);
			var enumerator = jsonNode.AsObject().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value != null)
					BuildDictionary(Depth, enumerator.Current.Value);
			}
		}
		else if (jsonNode is JsonArray)
		{
			var arr = jsonNode.AsArray();
			if (arr.All(x => x is null || x is JsonValue))
			{
				dict.Add(jsonNode.GetPath(), new ValueProviderResult(
					arr
					.Select(x => x?.GetValue<JsonElement>())
					.Select(y => y?.ValueKind == JsonValueKind.Number ? $"{y?.GetDouble().ToString(this.CultureInfo)}" : y?.ToString())
					.ToArray()));
			}
			else
			{
				dict.Add(jsonNode.GetPath(), ValueProviderResult.None);
				var parent_path = jsonNode.GetPath();
				var array = jsonNode.AsArray();
				for(int intIndex=0;intIndex< array.Count;intIndex++)
				{
					var arrayElement = array[intIndex];
					if (arrayElement != null)
						BuildDictionary(Depth, arrayElement);
					else // create path, because JsonNode == null has no path !!
						dict.Add($"{parent_path}[{intIndex}]", ValueProviderResult.None);
				}
			}
		}
		else if (jsonNode is JsonValue)
		{
			var path = jsonNode.GetPath();
			switch (jsonNode.GetValue<JsonElement>().ValueKind)
			{
				case JsonValueKind.Null: // never
					break;
				case JsonValueKind.Undefined: // never
					break;
				case JsonValueKind.Object: // never
					BuildDictionary(Depth, jsonNode);
					break;
				case JsonValueKind.Array: // never
					BuildDictionary(Depth, jsonNode);
					break;
				case JsonValueKind.String:
					dict.Add(path, new ValueProviderResult(new string[] { jsonNode.GetValue<string>() }));
					break;
				case JsonValueKind.Number:
					dict.Add(path, new ValueProviderResult(new string[] { jsonNode.GetValue<double>().ToString(this.CultureInfo) }));
					break;
				case JsonValueKind.True:
					dict.Add(path, new ValueProviderResult(new string[] { "true" }));
					break;
				case JsonValueKind.False:
					dict.Add(path, new ValueProviderResult(new string[] { "false" }));
					break;
			}
		}
		else
		{
			Debug.WriteLine($"Not Null, JsonObject, JsonArray, JsonValue Type:{jsonNode}");
		}

		Depth--;
		//Debug.WriteLine($"**{Depth}");
	}
#nullable disable
	public bool ContainsPrefix(string prefix)
	{
		var exists = dict.ContainsKey($"$.{prefix}");

		Debug.WriteLine($"ContainsPrefix('{prefix}') :: {exists}");

		return exists;
	}

	public ValueProviderResult GetValue(string key)
	{
		Debug.WriteLine($"GetValue('{key}')");

		if (dict.TryGetValue($"$.{key}", out ValueProviderResult val))
			return val;
		else
			return ValueProviderResult.None;
	}
}

public class JsonParametersValueProviderFactory : IValueProviderFactory
{
	private readonly CultureInfo CultureInfo;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, CultureInfo CultureInfo)
	{
		try
		{
			var jsonNode = await JsonSerializer.DeserializeAsync<JsonNode>(context.ActionContext.HttpContext.Request.Body);
			context.ValueProviders.Add(new JsonParametersValueProvider(jsonNode, CultureInfo));
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
	public JsonParametersValueProviderFactory(string CultureName)
	{
		this.CultureInfo = new CultureInfo(CultureName);
	}

	public JsonParametersValueProviderFactory()
	{
		this.CultureInfo = CultureInfo.InvariantCulture;
	}
}

