namespace KiDev.AndroidAutoFinder;

/// <summary>
/// Indicates that this element must be found on the <c>View</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class FindByIdAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="FindByIdAttribute"/> instance.
    /// </summary>
    /// <param name="id">Id to find this element by.</param>
    public FindByIdAttribute(int id) => Id = id;

    /// <summary>
    /// Id to find this element by.
    /// </summary>
    public int Id { get; private set; }
}