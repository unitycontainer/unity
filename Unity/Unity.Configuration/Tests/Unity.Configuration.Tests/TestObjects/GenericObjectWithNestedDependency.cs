namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class GenericObjectWithNestedDependency<T>
    {
        public GenericObjectWithConstructorDependency<T> Value { get; private set; }

        public GenericObjectWithNestedDependency(GenericObjectWithConstructorDependency<T> value)
        {
            Value = value;
        }
    }
}
