// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.ExceptionServices;

namespace ValueProviderBug;

/// <summary>
/// An <see cref="IModelBinder"/> for simple types.
/// </summary>
public class JsonModelBinder : IModelBinder
{
	private readonly Type type;

	/// <summary>
	/// Initializes a new instance of <see cref="SimpleTypeModelBinder"/>.
	/// </summary>
	/// <param name="type">The type to create binder for.</param>
	public JsonModelBinder(Type type)
	{
		this.type = type ?? throw new ArgumentNullException(nameof(type));
	}

	/// <inheritdoc />
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
			throw new ArgumentNullException(nameof(bindingContext));

		var compositeValueProvider = bindingContext.ValueProvider as CompositeValueProvider;

		if (compositeValueProvider == null)
			throw new ArgumentNullException(nameof(compositeValueProvider));

		if (compositeValueProvider.FirstOrDefault(x => x.GetType() == typeof(JsonValueProvider)) is not JsonValueProvider jsonValueProvider)
			throw new ArgumentNullException(nameof(jsonValueProvider));

		try
		{
			var objectValue = jsonValueProvider.GetObject(bindingContext.ModelName, this.type);

			bindingContext.Result = ModelBindingResult.Success(objectValue);

			return Task.CompletedTask;
		}
		catch (Exception exception)
		{
			var isFormatException = exception is FormatException;
			if (!isFormatException && exception.InnerException != null)
			{
				// TypeConverter throws System.Exception wrapping the FormatException,
				// so we capture the inner exception.
				exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
			}

			bindingContext.ModelState.TryAddModelError(
				bindingContext.ModelName,
				exception,
				bindingContext.ModelMetadata);

			// Were able to find a converter for the type but conversion failed.
			return Task.CompletedTask;
		}
	}

}

/// <summary>
/// An <see cref="IModelBinderProvider"/> for binding Json provided data types.
/// </summary>
public class JsonModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return new JsonModelBinder(context.Metadata.ModelType);
    }
}
