namespace FindViewById;

/// <summary>
/// Specifies that a specific <c>ContentView</c> needs to be set in this <c>Activity</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class SetViewAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="SetViewAttribute"/> instance.
    /// </summary>
    /// <param name="id">Id of <c>ContentView</c> to set.</param>
    public SetViewAttribute(int id) => Id = id;

    /// <summary>
    /// Id of <c>ContentView</c> to set.
    /// </summary>
    public int Id { get; private set; }
}