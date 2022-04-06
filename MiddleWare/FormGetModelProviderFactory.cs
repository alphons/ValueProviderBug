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
	public class FormGetModelProvider : BindingGetModelProvider
	{
		private readonly JsonSerializerOptions? jsonSerializerOptions;
		private readonly IFormCollection? form;

		public FormGetModelProvider(BindingSource bindingSource, IFormCollection form, JsonSerializerOptions? options) : base(bindingSource)
		{
			this.jsonSerializerOptions = options;

			this.form = form;
		}

		public override bool ContainsPrefix(string prefix)
		{
			if (this.form == null)
				return false;

			if (this.form.ContainsKey(prefix))
				return true;

			if(this.form.Files != null)
				return this.form.Files.Any(x => x.Name == prefix);

			return false;
		}

		/// <summary>
		/// Returns object of type when available
		/// </summary>
		/// <param name="key">name of the model</param>
		/// <param name="t">type of the model</param>
		/// <returns>null or object model of type</returns>
		public override object? GetModel(string key, Type t)
		{
			if (this.form == null)
				return null;

			if (this.form.ContainsKey(key))
			{
				var model = TypeDescriptor.GetConverter(t).ConvertFrom(
				   context: null,
				   culture: System.Globalization.CultureInfo.InvariantCulture,
				   value: this.form[key][0]); // Needs some tweaking
				return model;
			}

			if(this.form.Files != null && t == typeof(IFormFile))
				return this.form.Files.FirstOrDefault(x => x.Name == key);

			return null;
		}
	}


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
					
					context.ValueProviders.Add(new FormGetModelProvider(BindingSource.Form, form, options));
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


