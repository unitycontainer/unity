namespace ObjectBuilder2
{
    /// <summary>
    /// A <see cref="IBuilderPolicy"/> that controls how instances are
    /// persisted and recovered from an external store. Used to implement
    /// things like singletons and per-http-request lifetime.
    /// </summary>
    public interface ITransientPolicy : IBuilderPolicy
    {
    }
}
