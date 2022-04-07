using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestWeb;

public class SomeValueProviderFactory : IValueProviderFactory
{
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        context.ValueProviders.Add(new SomeValueProvider());
        return Task.CompletedTask;
    }
    public class SomeValueProvider : IValueProvider
    {
        public bool ContainsPrefix(string prefix)
        {
            return true;
        }
        public ValueProviderResult GetValue(string key)
        {
            return new ValueProviderResult(new string[] { "a", "b", null, "c" });
        }
    }

}

