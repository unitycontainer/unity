using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace GitHub
{
    // https://github.com/unitycontainer/unity/issues/54
    [TestClass]
    public class Issues
    {
        // http://www.codeplex.com/unity/WorkItem/View.aspx?WorkItemId=1307
        [TestMethod]
        public void StackOverflowException_54()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                container.RegisterType(typeof(ITestClass), typeof(TestClass));
                try
                {
//                    var instance = container.Resolve<ITestClass>(); //2
//                    Assert.IsNotNull(instance);
                }
                catch (Exception e)
                {
                    throw;
                }

            }
        }
    }
}
