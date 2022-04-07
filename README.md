# ValueProviderBug

This project was started as a demonstration of ValueProviderBug mentioned in https://github.com/dotnet/aspnetcore/issues/40929

Presented is a simple value provider, giving a linear one dimensional array of string values:
```c#
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
```

Controller method to check if value binding is correctly:

```c#
public async Task<IActionResult> Index(List<string> list)
{
  await Task.Yield();

  // Returned list "a", b", "b",  "c" <- repeat-bug
  // Expected list "a", b", null, "c"

  return Ok();
}
```
Also numbers are having the repeat bug, a javascript json post module is used (netproxy):
```javascript
 netproxy("./api/ComplexListNullableDouble", { list: [1.2, 3.4, 5.6, null, 7.8] });
```
To this controller method:
```c#
[HttpPost]
[Route("~/api/ComplexListNullableDouble")]
public async Task<IActionResult> ComplexListNullableDouble(List<double?> list)
{
	await Task.Yield();
	return Ok();
}
```
Results in the repeat bug: 1.2, 3.4, 5.6, **5.6**, 7.8

Turns out this is due to the CollectionModelBinderProvider in combination with SimpleTypeModelBinderProvider:

```c#
builder.Services.AddMvcCore().AddMvcOptions(options =>
{
 options.OutputFormatters.Clear();
 options.InputFormatters.Clear();
 options.ValueProviderFactories.Clear();
 options.ModelValidatorProviders.Clear();
 options.Conventions.Clear();
 options.Filters.Clear();
 options.ModelMetadataDetailsProviders.Clear();
 options.ModelValidatorProviders.Clear();
 options.ModelMetadataDetailsProviders.Clear();
 options.ModelBinderProviders.Clear();

options.ModelBinderProviders.Add(new CollectionModelBinderProvider()); // Repeat previous value on null value

options.ModelBinderProviders.Add(new SimpleTypeModelBinderProvider()); 
options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory()); // See top of this article
});
```

Posting a json array of an array of strings to a controller:

```javascript
{
  "Users":
  [
    ["admins", "0"],
    ["editors", "1"],
    null,
    ["sisters", "3"]
  ]
}
``` 
Using this controller 
```c#
[HttpPost]
[Route("~/api/ComplexArrayTruncatedBuf")]
public async Task<IActionResult> ComplexArrayTruncatedBuf(List<List<string>> Users)
{
  await Task.Yield();
  return Ok(new
  {
    Users
  });
}
```
Results in a truncated array, where only items before the null value are returned.

**problem**

The real problem is the MVC internal `ValueProviderResult` which is a wrapper around string values. 
A special value is reserverd for `ValueProviderResult.None`
```c#
public static ValueProviderResult None = new ValueProviderResult(Array.Empty<string>());
```

more to come
