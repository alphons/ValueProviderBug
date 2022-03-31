using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestWeb
{
	public class SimpleTypeNoNullModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (!context.Metadata.IsComplexType)
				return new SimpleTypeNoNullModelBinder(context.Metadata.ModelType);
			else
				return null;
		}
	}

	public class SimpleTypeNoNullModelBinder : IModelBinder
	{
		private readonly TypeConverter _typeConverter;

		/// <summary>
		/// Initializes a new instance of <see cref="SimpleTypeNoNullModelBinder"/>.
		/// </summary>
		/// <param name="type">The type to create binder for.</param>
		public SimpleTypeNoNullModelBinder(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			_typeConverter = TypeDescriptor.GetConverter(type);
		}

		/// <inheritdoc />
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueProviderResult == ValueProviderResult.None)
			{
				return Task.CompletedTask;
			}

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

			try
			{
				var value = valueProviderResult.FirstValue;

				//Debug.WriteLine($"[{value}]");

				object? model;
				if (bindingContext.ModelType == typeof(string))
				{
					model = value; // "" != null
				}
				else if (string.IsNullOrWhiteSpace(value))
				{
					// Other than the StringConverter, converters Trim() the value then throw if the result is empty.
					model = null;
				}
				else
				{
					model = _typeConverter.ConvertFrom(
						context: null,
						culture: valueProviderResult.Culture,
						value: value);
				}

				CheckModel(bindingContext, valueProviderResult, model);

				return Task.CompletedTask;
			}
			catch (Exception exception)
			{
				var isFormatException = exception is FormatException;
				if (!isFormatException && exception.InnerException != null)
				{
					// TypeConverter throws System.Exception wrapping the FormatException,
					// so we capture the inner exception.
					exception = exception.InnerException;
				}

				bindingContext.ModelState.TryAddModelError(
					bindingContext.ModelName,
					exception,
					bindingContext.ModelMetadata);

				// Were able to find a converter for the type but conversion failed.
				return Task.CompletedTask;
			}
		}

		/// <summary>
		/// If the <paramref name="model" /> is <see langword="null" />, verifies that it is allowed to be <see langword="null" />,
		/// otherwise notifies the <see cref="P:ModelBindingContext.ModelState" /> about the invalid <paramref name="valueProviderResult" />.
		/// Sets the <see href="P:ModelBindingContext.Result" /> to the <paramref name="model" /> if successful.
		/// </summary>
		protected virtual void CheckModel(
			ModelBindingContext bindingContext,
			ValueProviderResult valueProviderResult,
			object? model)
		{
			// When converting newModel a null value may indicate a failed conversion for an otherwise required
			// model (can't set a ValueType to null). This detects if a null model value is acceptable given the
			// current bindingContext. If not, an error is logged.
			if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
			{
				bindingContext.ModelState.TryAddModelError(
					bindingContext.ModelName,
					bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
						valueProviderResult.ToString()));
			}
			else
			{
				bindingContext.Result = ModelBindingResult.Success(model);
			}
		}
	}
}
