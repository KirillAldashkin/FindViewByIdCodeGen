namespace KiDev.AndroidAutoFinder;

/// <summary>
/// Uses method as a <c>TextChanged</c> event listener for a specified view.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class OnTextChangedAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="OnTextChangedAttribute"/> instance.
    /// </summary>
    /// <param name="id">Id of view to set listener on.</param>
    public OnTextChangedAttribute(int id) => Id = id;

    /// <summary>
    /// Id of view to set listener on.
    /// </summary>
    public int Id { get; private set; }
}
