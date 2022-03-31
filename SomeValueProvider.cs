

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestWeb
{
	public class SomeValueProvider : IValueProvider
	{
		public bool ContainsPrefix(string prefix)
		{
			return true;
		}
		public ValueProviderResult GetValue(string key)
		{
			return new ValueProviderResult(new string[] { "a", "", "b", null, "c" }); // "a", null, "b", "b", "c"
			//return new ValueProviderResult(new string[] { "a", "b", null, "c" }); // "a", "b", "b", "c" => null, repeats "b"
			//return new ValueProviderResult(new string[] { null, "a", "b", null, "c" }); // "a", "b", "b", "c" first null skipped
		}
	}

	public class SomeValueProviderFactory : IValueProviderFactory
	{
		private static async Task AddValueProviderAsync(ValueProviderFactoryContext context)
		{
			await Task.Yield();
			context.ValueProviders.Add(new SomeValueProvider());
		}

		public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
		{
			return AddValueProviderAsync(context);
		}
	}



}

