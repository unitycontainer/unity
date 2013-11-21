namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public interface IInterfaceWithGenericMethod
    {
        [TestHandler]
        T DoSomething<T>();
    }
}