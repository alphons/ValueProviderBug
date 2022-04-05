// GenericModelBinderProvider, GenericModelBinder
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

#nullable enable

using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

public interface IGetModelProvider
{
	object? GetModel(string key, Type t);
	bool ContainsPrefix(string prefix);
}


/// <summary>
/// An <see cref="IGetModelProvider"/> sufficient for most objects.
/// </summary>
public class GenericModelBinder : IModelBinder
{
	private readonly Type type;

	/// <summary>
	/// Initializes a new instance of <see cref="GenericModelBinder"/>.
	/// </summary>
	/// <param name="type">The type to create binder for.</param>
	public GenericModelBinder(Type type)
	{
		this.type = type ?? throw new ArgumentNullException(nameof(type));
	}

	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
			throw new ArgumentNullException(nameof(bindingContext));

		var compositeValueProvider = bindingContext.ValueProvider as CompositeValueProvider;

		if (compositeValueProvider == null)
			throw new ArgumentNullException(nameof(compositeValueProvider));

		if (compositeValueProvider.FirstOrDefault(x => x is IGetModelProvider) is not IGetModelProvider getModelProvider)
			throw new ArgumentNullException(nameof(getModelProvider));

		try
		{
			var model = getModelProvider.GetModel(bindingContext.ModelName, this.type);

			bindingContext.Result = ModelBindingResult.Success(model);

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
/// An <see cref="IModelBinderProvider"/> a GenericModelBinderProvider for the famous GenericModelBinder.
/// </summary>
public class GenericModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return new GenericModelBinder(context.Metadata.ModelType);
    }
}
