namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// This is a custom lifetime manager that acts like <see cref="TransientLifetimeManager"/>,
    /// but also provides a signal to the default build plan, marking the type so that
    /// instances are reused across the build up object graph.
    /// </summary>
    public class PerBuildSingleton : TransientLifetimeManager
    {
        private readonly object value;

        /// <summary>
        /// Construct a new <see cref="PerBuildSingleton"/> object that does not
        /// itself manage an instance.
        /// </summary>
        public PerBuildSingleton()
        {
        }

        /// <summary>
        /// Construct a new <see cref="PerBuildSingleton"/> object that stores the
        /// give value. This value will be returned by <see cref="LifetimeManager.GetValue"/>
        /// but is not stored in the lifetime manager, nor is the value disposed.
        /// This Lifetime manager is intended only for internal use, which is why the
        /// normal <see cref="LifetimeManager.SetValue"/> method is not used here.
        /// </summary>
        /// <param name="value">Value to store.</param>
        internal PerBuildSingleton(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object GetValue()
        {
            return value;
        }
    }
}