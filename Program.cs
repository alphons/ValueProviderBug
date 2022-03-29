
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvcCore().AddMvcOptions(options =>
{
	options.ValueProviderFactories.Insert(0, new TestWeb.SomeValueProviderFactory());
});

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
