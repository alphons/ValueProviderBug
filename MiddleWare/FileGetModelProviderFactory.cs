using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

#nullable enable
public class FileGetModelProviderFactory : IValueProviderFactory
{
	public class FileGetModelProvider : IGetModelProvider, IValueProvider // IValueProvider for compatibility reasons
	{
		private readonly JsonSerializerOptions? jsonSerializerOptions;
		private readonly IFormCollection? form;

		public FileGetModelProvider(IFormCollection form, JsonSerializerOptions? options)
		{
			this.jsonSerializerOptions = options;

			this.form = form;
		}

		public bool ContainsPrefix(string prefix)
		{
			if (this.form == null || this.form.Files == null)
				return false;

			return this.form.Files.Any(x => x.Name == prefix);
		}

		/// <summary>
		/// Returns object of type when available
		/// </summary>
		/// <param name="key">name of the model</param>
		/// <param name="t">type of the model</param>
		/// <returns>null or object model of type</returns>
		public object? GetModel(string key, Type t)
		{
			if (this.form != null && this.form.Files != null)
			{
				var file = this.form.Files.FirstOrDefault(x => x.Name == key);
				return file;
			}
			return null;
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
					
					context.ValueProviders.Add(new FileGetModelProvider(form, options));
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
	public FileGetModelProviderFactory(JsonSerializerOptions Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public FileGetModelProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

}


