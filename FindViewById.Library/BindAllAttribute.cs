namespace KiDev.AndroidAutoFinder;

/// <summary>
/// Binds a layout and all elements in it.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BindAllAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="BindAllAttribute"/> instance.
    /// </summary>
    /// <param name="id">Id of layout file. <b>Use explicit resource id string, like: </b><c>Resource.Layout.activity_main</c></param>
    public BindAllAttribute(int id) => Id = id;

    /// <summary>
    /// Id of layout file.
    /// </summary>
    public int Id { get; private set; }
}