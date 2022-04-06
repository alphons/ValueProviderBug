
// CookyValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-06
// Version: 1.1

using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Heijden.AspNetCore.Mvc.ModelBinding;

#nullable enable
public class CookyValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        var cookies = context.ActionContext.HttpContext.Request.Cookies;
        if (cookies != null && cookies.Count > 0)
        {
            var list = cookies.Select(x => $"\"{x.Key}\": \"{x.Value}\"").ToArray();
            var json = $"{{{string.Join(',', list)}}}";
            var jsonDocument = JsonDocument.Parse(json, options: default);

            var valueProvider = new GenericValueProvider(
                BindingSource.Special,
                jsonDocument,
                null,
                jsonSerializerOptions);

            context.ValueProviders.Add(valueProvider);
        }

        return Task.CompletedTask;
    }

	public CookyValueProviderFactory(JsonSerializerOptions Options)
	{
		this.jsonSerializerOptions = Options;
	}

	public CookyValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


