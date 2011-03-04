namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class ClassWithGenericMethod : IInterfaceWithGenericMethod
    {
        public T DoSomething<T>()
        {
            return default(T);
        }
    }
}