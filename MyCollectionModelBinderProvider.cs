
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestWeb
{
	public class MyCollectionModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			return new MyCollectionModelBinder();
		}
	}

	public class MyCollectionModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType != typeof(List<string>))
				return Task.CompletedTask;

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

			bindingContext.Result = ModelBindingResult.Success(valueProviderResult.ToList());

			return Task.CompletedTask;
		}

		
	}
}
