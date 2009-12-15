namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// An element that has a child Value property.
    /// </summary>
    public interface IValueProvidingElement
    {
        /// <summary>
        /// String that will be deserialized to provide the value.
        /// </summary>
        ParameterValueElement Value { get; set; }

        /// <summary>
        /// A string describing where the value this element contains
        /// is being used. For example, if setting a property Prop1,
        /// this should return "property Prop1" (in english).
        /// </summary>
        string DestinationName { get; }
    }
}