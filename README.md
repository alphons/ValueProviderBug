# ASP.NET CORE MVC Parameter Binding Fundamentaly Flawed

This project was started as a demonstration of problems as mentioned in https://github.com/dotnet/aspnetcore/issues/40929

## Problem 1: CollectionModelBinderProvider/SimpleTypeModelBinderProvider have null repeat bug

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
Setup:
```c#
builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	options.InputFormatters.Clear();
	options.ValueProviderFactories.Clear();
	options.ModelValidatorProviders.Clear();
	options.Conventions.Clear();
	options.Filters.Clear();
	options.ModelMetadataDetailsProviders.Clear();
	options.ModelValidatorProviders.Clear();
	options.ModelMetadataDetailsProviders.Clear();
	options.ModelBinderProviders.Clear();
	options.OutputFormatters.Clear();

	options.ModelBinderProviders.Add(new CollectionModelBinderProvider());
	options.ModelBinderProviders.Add(new SimpleTypeModelBinderProvider());

	options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory());
});
```
Controller method to check if value binding is correctly:

```c#
[HttpGet]
[Route("~/api/index")]
public async Task<IActionResult> Index(List<string> list)
{
  await Task.Yield();

  // Using SomeValueProviderFactory
  // Returned list "a", b", "b",  "c" <- repeat-bug on null values
  // Expected list "a", b", null, "c"

  return Ok();
}
```

## Problem 2: Json posts can only be bind when using [FromBody]

### 2.1 Testing Single string

For testing we use a small javascript module `netproxy` which can do POST requests of Json data.

```javascript
	var user = { user: 'alphons' };
	r = await netproxyasync("./api/SimpleString1", user);
	r = await netproxyasync("./api/SimpleString2", user);
	r = await netproxyasync("./api/SimpleString3", user);
	r = await netproxyasync("./api/SimpleString4", user);
```
Most default setup:
```c#
builder.Services.AddMvcCore();
```
Controller methods used:
```c#
[HttpPost]
[Route("~/api/SimpleString1")]
public async Task<IActionResult> SimpleString1(string user)
{
	await Task.Yield();
	return Ok(new
	{
		user
	});
}

[HttpPost]
[Route("~/api/SimpleString2")]
public async Task<IActionResult> SimpleString2([FromBody] string user)
{
	await Task.Yield();
	return Ok(new
	{
		user
	});
}

public class ModelOfString
{
	public string user { get; set; }
}

[HttpPost]
[Route("~/api/SimpleString3")]
public async Task<IActionResult> SimpleString3(ModelOfString model)
{
	await Task.Yield();
	return Ok(new
	{
		model.user
	});
}

[HttpPost]
[Route("~/api/SimpleString4")]
public async Task<IActionResult> SimpleString4([FromBody] ModelOfString model)
{
	await Task.Yield();
	return Ok(new
	{
		model?.user
	});
}
```
In this default Mvc setup, only `SimpleString4([FromBody] ModelOfString model)` has succes!

### 2.2 Testing Array of Strings

Lets try an array of strings:
```javascript
var users = { "users": ["admins", "editors", null, "sisters"] };

r = await netproxyasync("./api/ArrayOfStrings1", users);
r = await netproxyasync("./api/ArrayOfStrings2", users);
r = await netproxyasync("./api/ArrayOfStrings3", users);
r = await netproxyasync("./api/ArrayOfStrings4", users);
```
Controller methods:
```c#
[HttpPost]
[Route("~/api/ArrayOfStrings1")]
public async Task<IActionResult> ArrayOfStrings1(List<string> users)
{
	await Task.Yield();
	return Ok(new
	{
		users
	});
}

[HttpPost]
[Route("~/api/ArrayOfStrings2")]
public async Task<IActionResult> ArrayOfStrings2([FromBody] List<string> users)
{
	await Task.Yield();
	return Ok(new
	{
		users
	});
}

public class ModelArrayOfStrings
{
	public List<string> users { get; set; }
}

[HttpPost]
[Route("~/api/ArrayOfStrings3")]
public async Task<IActionResult> ArrayOfStrings3(ModelArrayOfStrings model)
{
	await Task.Yield();
	return Ok(new
	{
		model.users
	});
}

[HttpPost]
[Route("~/api/ArrayOfStrings4")]
public async Task<IActionResult> ArrayOfStrings4([FromBody] ModelArrayOfStrings model)
{
	await Task.Yield();
	return Ok(new
	{
		model?.users
	});
}
```
Same result, only `ArrayOfStrings4([FromBody] ModelArrayOfStrings model)` does work!

### 2.3 Testing Array Of Array Of Strings

```javascript
var arrayofarrayofstrings = { "Users": [ ["admins", "0"], ["editors", "1"], null, ["sisters", "3"] ] };
r = await netproxyasync("./api/ArrayOfArrayOfStrings1", arrayofarrayofstrings);
r = await netproxyasync("./api/ArrayOfArrayOfStrings2", arrayofarrayofstrings);
```
Controller methods:
```c#
public class ModelArrayOfArrayOfStrings
{
	public List<List<string>> users { get; set; }
}

[HttpPost]
[Route("~/api/ArrayOfArrayOfStrings1")]
public async Task<IActionResult> ArrayOfArrayOfStrings1(ModelArrayOfArrayOfStrings model)
{
	await Task.Yield();
	return Ok(new
	{
		model.users
	});
}

[HttpPost]
[Route("~/api/ArrayOfArrayOfStrings2")]
public async Task<IActionResult> ArrayOfArrayOfStrings2([FromBody] ModelArrayOfArrayOfStrings model)
{
	await Task.Yield();
	return Ok(new
	{
		model.users
	});
}
```
Resulting only success for `ArrayOfArrayOfStrings2([FromBody] ModelArrayOfArrayOfStrings model)`

## Problem 3 Only binding to 1 parameter of a controller method

Because of the nature of [FromBody], which uses `SystemTextJsonInputFormatter` this can only work when using only 1 parameter.

These will always fail!

```javascript
r = await netproxyasync("./api/TwoParameters1", { a: 'aa aa', b: 'bbb bbb' });
r = await netproxyasync("./api/TwoParameters2", { a: 'aa aa', b: 'bbb bbb' });
r = await netproxyasync("./api/TwoParameters3", { a: 'aa aa', b: 'bbb bbb' });
```
Controller methods:
```c#
[HttpPost]
[Route("~/api/TwoParameters1")]
public async Task<IActionResult> TwoParameters1(string a, string b)
{
	await Task.Yield();
	return Ok(new
	{
		a,
		b
	});
}

[HttpPost]
[Route("~/api/TwoParameters2")]
public async Task<IActionResult> TwoParameters2([FromBody] string a, string b)
{
	await Task.Yield();
	return Ok(new
	{
		a,b
	});
}

[HttpPost]
[Route("~/api/TwoParameters3")]
public async Task<IActionResult> TwoParameters3([FromBody] ModelOfString a, ModelOfString b)
{
	await Task.Yield();
	return Ok(new
	{
		a = a?.user,
		b = b?.user
	});
}
```

## Solution 1 Implementing a JsonParametersValueProvider

By implementing a json parameter value provider we can bind any number of parameters except 
parameters which have the [FromBody] attribute.

### Solution 1 Problem, repeater bug on null values when using Custom ValueProvider

```javascript
var users = { "users": ["admins", "editors", null, "sisters"] };
r = await netproxyasync("./api/ArrayOfStrings1", users);
```
Posted as json to this controller method:
```c#
[HttpPost]
[Route("~/api/ArrayOfStrings1")]
public async Task<IActionResult> ArrayOfStrings1(List<string> users)
{
	await Task.Yield();
	return Ok(new
	{
		users
	});
}
```
Results in following data:
```javascript
{ "users": ["admins", "editors", "editors", "sisters"] }
```
The null value is replaced by the previous value (this case 'editors').

A nother check if it is not a 'string' thingy.
```javascript
 r = await netproxyasync("./api/ListOfDoubles", { list: [1.2, 3.4, 5.6, null, 7.8] });
```
To this controller method:
```c#
[HttpPost]
[Route("~/api/ListOfDoubles")]
public async Task<IActionResult> ComplexListNullableDouble(List<double?> list)
{
	await Task.Yield();
	return Ok();
}
```
Results also in the repeat bug: 1.2, 3.4, 5.6, **5.6**, 7.8

## Solution 1 Conclusion, current achitecture is flawed and error prone

Only the array elements prior to the null element are binded to the parameter of the controller method.
And that can NOT be fixed because of the nature of the current binding. So it is not a problem of the
CollectionModelBinder, but it is a general problem of the current valueproviding en modelbinding.
I hate to say this, but **this is rotten to the core**!

## Some lesson in valueproviding and modelbinding

First, some short lesson in current .net core valueproviding and modelbinding.

For multi-parameter binding we can not use the default `SystemTextJsonInputFormatter` because the formatter can only
read input (once) and deliver 1 type of model to the binder.

So for multi-parameter binding, we have to use one or more valueproviders which in fact can deliver 
multiple types by its parameter name.

All input data is deserialized using a valueprovider of choosing. Every valueprovider must
implement the `IValueProvider` interface, which has 2 methods, `bool ContainsPrefix(string prefix)` and
`ValueProviderResult GetValue(string key)`. And thats the place where it gets a bit smelly. The `ValueProviderResult`
is some kind of wrapper arround an array of strings, thats it.

So a fundamental architectual error is made her. The valueprovider does the first part of the deserialization of the data, 
and the binder (or set of nested binders if you like), are doing the second part of deserialization (?!)

When looking at the process of value-provider and binder, for a more complex input data structure for example
having multiple hierarchical layers (array of array of array):

```javascript
{
  "SomeParameter4": // Now the beast has a name
  {
    Name: "My Name is",
    "Users":
    [
      [{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
      [{ Name: "User10" }, { Name: "User11" }],
      [{ Name: "User20" }, { Name: "User21" }]
    ]
  },
  "SomeParameter5": "Yes you can" // double binder
}
```
These calls are made to a ValueProvider:

```
ContainsPrefix('SomeParameter4') :: True
GetValue('SomeParameter4.Name')
ContainsPrefix('SomeParameter4.Users') :: True
GetValue('SomeParameter4.Users')
GetValue('SomeParameter4.Users.index')
ContainsPrefix('SomeParameter4.Users[0]') :: True
GetValue('SomeParameter4.Users[0]')
GetValue('SomeParameter4.Users[0].index')
ContainsPrefix('SomeParameter4.Users[0][0].Name') :: True
GetValue('SomeParameter4.Users[0][0].Name')
ContainsPrefix('SomeParameter4.Users[0][0].Alias') :: True
GetValue('SomeParameter4.Users[0][0].Alias')
ContainsPrefix('SomeParameter4.Users[0][1].Name') :: True
GetValue('SomeParameter4.Users[0][1].Name')
ContainsPrefix('SomeParameter4.Users[0][1].Alias') :: False
ContainsPrefix('SomeParameter4.Users[0][2].Name') :: False
ContainsPrefix('SomeParameter4.Users[0][2].Alias') :: False
ContainsPrefix('SomeParameter4.Users[1]') :: True
GetValue('SomeParameter4.Users[1]')
GetValue('SomeParameter4.Users[1].index')
ContainsPrefix('SomeParameter4.Users[1][0].Name') :: True
GetValue('SomeParameter4.Users[1][0].Name')
ContainsPrefix('SomeParameter4.Users[1][0].Alias') :: False
ContainsPrefix('SomeParameter4.Users[1][1].Name') :: True
GetValue('SomeParameter4.Users[1][1].Name')
ContainsPrefix('SomeParameter4.Users[1][1].Alias') :: False
ContainsPrefix('SomeParameter4.Users[1][2].Name') :: False
ContainsPrefix('SomeParameter4.Users[1][2].Alias') :: False
ContainsPrefix('SomeParameter4.Users[2]') :: True
GetValue('SomeParameter4.Users[2]')
GetValue('SomeParameter4.Users[2].index')
ContainsPrefix('SomeParameter4.Users[2][0].Name') :: True
GetValue('SomeParameter4.Users[2][0].Name')
ContainsPrefix('SomeParameter4.Users[2][0].Alias') :: False
ContainsPrefix('SomeParameter4.Users[2][1].Name') :: True
GetValue('SomeParameter4.Users[2][1].Name')
ContainsPrefix('SomeParameter4.Users[2][1].Alias') :: False
ContainsPrefix('SomeParameter4.Users[2][2].Name') :: False
ContainsPrefix('SomeParameter4.Users[2][2].Alias') :: False
ContainsPrefix('SomeParameter4.Users[3]') :: False
ContainsPrefix('SomeParameter5') :: True
GetValue('SomeParameter5')
```

And every returned value from `GetValue` is an array of strings.
So there is some shadowy type converting which takes place in one or more (nested) binders. 
You have no influence on this translation path.

**In the end, its like a box of chocolates....... **

## Proposal, adding GetModel to IBindingSourceValueProvider

I want to propose an additional method to the `IBindingSourceValueProvider` interface:  
```c#
object? GetModel(string key, Type t)
```
The class which implements this interface `BindingSourceValueProvider` can have a virtual method:
```c#
public virtual object? GetModel(string key, Type t)
{
	return null;
}
```

And there is a huge advantage. The translation between input values and the model it has to provide can be done INSIDE the ValueProvider. Thas the place where it belongs, not in the binder.

The binder for such a model can ben very simple. It does call the `ContainsPrefix` on every parameter, but does NOT NEED recursively calls, no additional requests are made, even when the model itself is complex and had some 'depth' in it.
For example when a controller has a method having 4 parameters, the binder is called only 4 times independently of its complex model structure per parameter.
The core of the new ModelBinder is following code:
```c#
try
{
  var model = getModelProvider.GetModel(defaultContext.OriginalModelName, this.type);
  bindingContext.Result = ModelBindingResult.Success(model);
  return Task.CompletedTask;
}
...
...
```
**Example**

To have some proof, the following setup is used:
```c#
services.AddMvcCore().AddMvcOptions(options =>
{
  options.InputFormatters.Clear();
  options.ValueProviderFactories.Clear();
  options.ModelValidatorProviders.Clear();
  options.Conventions.Clear();
  options.Filters.Clear();
  options.ModelMetadataDetailsProviders.Clear();
  options.ModelValidatorProviders.Clear();
  options.ModelMetadataDetailsProviders.Clear();
  options.ModelBinderProviders.Clear();
  options.OutputFormatters.Clear();

  // Adding our new custom valueproviders (all implementing GetModel)
  options.ValueProviderFactories.Add(new JsonValueProviderFactory());
  options.ValueProviderFactories.Add(new HeaderValueProviderFactory());
  options.ValueProviderFactories.Add(new CookyValueProviderFactory());

  options.ValueProviderFactories.Add(new QueryStringValueProviderFactory());
  options.ValueProviderFactories.Add(new RouteValueProviderFactory());
  options.ValueProviderFactories.Add(new FormFileValueProviderFactory());
  options.ValueProviderFactories.Add(new FormValueProviderFactory());	

  // One Generic binder gettings complete de-serialized calling GetModel(name,type)
  options.ModelBinderProviders.Add(new GenericModelBinderProvider());
}
```
When using thesame input data, these calls are made to the new valueprovider implementing `GetModel`.
```
ContainsPrefix(SomeParameter4)
ContainsPrefix(SomeParameter4)
GetModel(SomeParameter4)
ContainsPrefix(SomeParameter5)
ContainsPrefix(SomeParameter5)
GetModel(SomeParameter5)
```

Now this example can never work on the current .net core implementation but works out-of-the-box using the new proposal.
(also multiple [FromBody])
```c#
[HttpPost]
[Route("~/api/DemoProposal/{SomeParameter2}")]
public async Task<IActionResult> DemoProposal(
	[FromCooky(Name = ".AspNetCore.Session")] string SomeParameter0, // #######
	[FromHeader(Name = "Referer")] string SomeParameter1,	// "https://localhost:44346/"
	[FromRoute] string SomeParameter2,			// "two"
	[FromQuery] string SomeParameter3,			// "three"
	[FromBody] ApiModel SomeParameter4,			//  Model having Array of Array of Array
	[FromBody] string SomeParameter5)			//  "Yes you can" (double binding FromBody)
{
	await Task.Yield();
	return Ok();
}
```

End of proposal







