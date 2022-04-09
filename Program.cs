
// nuget package Mvc.ModelBinding.MultiParameter
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

var builder = WebApplication.CreateBuilder();

// Problem 1: For demonstrating the repeat bug, call /api/index
// ======================================================================
//builder.Services.AddMvcCore()
//	.AddMvcOptions(options =>
//	{
//		options.InputFormatters.Clear();
//		options.ValueProviderFactories.Clear();
//		options.ModelValidatorProviders.Clear();
//		options.Conventions.Clear();
//		options.Filters.Clear();
//		options.ModelMetadataDetailsProviders.Clear();
//		options.ModelValidatorProviders.Clear();
//		options.ModelMetadataDetailsProviders.Clear();
//		options.ModelBinderProviders.Clear();
//		options.OutputFormatters.Clear();

//		options.ModelBinderProviders.Add(new CollectionModelBinderProvider());
//		options.ModelBinderProviders.Add(new SimpleTypeModelBinderProvider());

//		options.ValueProviderFactories.Add(new TestWeb.SomeValueProviderFactory());
//	});
// ======================================================================

// Problem 2/3: Json POST data can only be binded by a class model
// ======================================================================
//builder.Services.AddMvcCore();
// ======================================================================

//Solution 1: Json Parameter value provider, can not be used by [FromBody]
// ======================================================================
//builder.Services.AddMvcCore()
//	.AddMvcOptions(options =>
//	{
//		options.ValueProviderFactories.Insert(0, new JsonParametersValueProviderFactory());
//	});
// ======================================================================

//Proposal: ValueProviders implementing GetModel(name, type);
// ======================================================================
builder.Services.AddMvcCoreCorrected();
// ======================================================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.UseSession();
app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
