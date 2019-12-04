namespace Sample.Views
{
    public interface IBindablePage
    {
        /// <summary>
        /// Gets or sets the binding context.
        /// </summary>
        object BindingContext { get; set; }
    }
}