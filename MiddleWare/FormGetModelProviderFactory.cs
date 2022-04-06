using System.ComponentModel;
using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

#nullable enable
public class FormGetModelProviderFactory : IValueProviderFactory
{

	private readonly JsonSerializerOptions? jsonSerializerOptions;

	private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, JsonSerializerOptions? options)
	{
		try
		{
			var request = context.ActionContext.HttpContext.Request;
			if (request.Method == "POST")
			{
				if (request.HasFormContentType)
				{
					var form = await request.ReadFormAsync();
					
					context.ValueProviders.Add(new GetModelProvider(BindingSource.Form, null, form, options));
				}
			}
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
	public FormGetModelProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public FormGetModelProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


