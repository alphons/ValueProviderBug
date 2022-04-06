#nullable enable

//
// Summary:
//     Specifies that a parameter or property should be bound using the request cookies.
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Heijden.AspNetCore.Mvc.ModelBinding;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class FromCookyAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider, IFromHeaderMetadata
{
	public BindingSource BindingSource
	{
		get
		{
			return BindingSource.Special;
		}
	}

	public string? Name { get; set; }

	public FromCookyAttribute()
	{
	}
}