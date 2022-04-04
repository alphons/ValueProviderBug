

using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace TestWeb
{
	public class SomeValueProviderFactory : IValueProviderFactory
	{
		private class SomeValueProvider : IValueProvider
		{
			public bool ContainsPrefix(string prefix)
			{
				if(prefix == "ListOfStrings")
					return true;
				if (prefix == "ListOfInts")
					return true;
				if (prefix == "list")
					return true;
				if (prefix == "list[0].a" || prefix == "list[0].b")
					return true;
				if (prefix == "list[1].a" || prefix == "list[1].b")
					return false; // null
				if (prefix == "list[2].a" || prefix == "list[2].b")
					return true;
				return false;
			}
			// emulate  { ListOfStrings: ['a', '', 'b', null, 'c'] } => "a", null, "b", "b", "c" (repeat bug)
			// emulate  { ListOfInts: [ 0, 1 , 2, null , 4] } =>   null, 1, 2, 2, 4 (repeat bug)
			// emulate  { list: [{ a: 'a', b: null }, null, { a: 'c', b: 'd' }] }); => list: [{ a: 'a', b: null } ] , null value is end array
			public ValueProviderResult GetValue(string key)
			{
				if(key == "ListOfStrings")
					return new ValueProviderResult(new string[] { "a", "", "b", null, "c" }); // "a", null, "b", "b", "c"
				if (key == "ListOfInts")
					return new ValueProviderResult(new string[] { "", "1", "2", null, "4" }); // null, 1, 2, 2, 4

				// array element 0
				if(key == "list[0].a")
						return new ValueProviderResult(new string[] { "a" });
				if (key == "list[0].b")
					return new ValueProviderResult(new string[] { null });

				// array element 1 = null

				// array element 2
				if (key == "list[2].a")
					return new ValueProviderResult(new string[] { "c" });
				if (key == "list[2].b")
					return new ValueProviderResult(new string[] { "d" });

				return ValueProviderResult.None; // list ans list.index are  ValueProviderResult.None
			}
		}

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

