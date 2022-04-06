
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alternative.DependencyInjection;

#nullable enable

public abstract class BindingGetModelProvider : IBindingGetModelProvider
{
    /// <summary>
    /// Creates a new <see cref="BindingSourceValueProvider"/>.
    /// </summary>
    /// <param name="bindingSource">
    /// The <see cref="ModelBinding.BindingSource"/>. Must be a single-source (non-composite) with
    /// <see cref="ModelBinding.BindingSource.IsGreedy"/> equal to <c>false</c>.
    /// </param>
    public BindingGetModelProvider(BindingSource bindingSource)
    {
        if (bindingSource == null)
        {
            throw new ArgumentNullException(nameof(bindingSource));
        }

        if (bindingSource.IsGreedy)
        {
            throw new ArgumentException("CannotBeGreedy", nameof(bindingSource));
        }

        if (bindingSource is CompositeBindingSource)
        {
            throw new ArgumentException("CannotBeComposite", nameof(bindingSource));
        }

        BindingSource = bindingSource;
    }

    /// <summary>
    /// Gets the corresponding <see cref="ModelBinding.BindingSource"/>.
    /// </summary>
    protected BindingSource BindingSource { get; }

    /// <inheritdoc />
    public abstract bool ContainsPrefix(string prefix);

    /// <inheritdoc />
    public ValueProviderResult GetValue(string key)
	{
        return ValueProviderResult.None;

    }

    /// <inheritdoc />
    public virtual IGetModelProvider? Filter(BindingSource bindingSource)
    {
        if (bindingSource == null)
        {
            throw new ArgumentNullException(nameof(bindingSource));
        }

        if (bindingSource.CanAcceptDataFrom(BindingSource))
        {
            return (IGetModelProvider?)this;
        }
        else
        {
            return null;
        }
    }
}

