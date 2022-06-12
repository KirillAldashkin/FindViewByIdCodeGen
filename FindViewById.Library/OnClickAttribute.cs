namespace KiDev.AndroidAutoFinder;

/// <summary>
/// Uses method as a <c>Click</c> event listener for a specified view.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class OnClickAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="OnClickAttribute"/> instance.
    /// </summary>
    /// <param name="id">Id of view to set listener on.</param>
    public OnClickAttribute(int id) => Id = id;

    /// <summary>
    /// Id of view to set listener on.
    /// </summary>
    public int Id { get; private set; }
}
