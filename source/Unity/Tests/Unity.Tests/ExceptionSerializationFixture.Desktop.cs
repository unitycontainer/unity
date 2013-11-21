// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class ExceptionSerializationFixture
    {
        [TestMethod]
        public void ResolutionFailedExceptionIsSerializedAcrossAppDomainBoundaries()
        {
            var domain =
                AppDomain.CreateDomain("test", null, new AppDomainSetup { ApplicationBase = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) });

            try
            {
                domain.DoCallBack(() =>
                    {
                        var container = new UnityContainer();
                        container.Resolve<IDisposable>("test");
                    });
            }
            catch (ResolutionFailedException e)
            {
                Assert.AreEqual("test", e.NameRequested);
                Assert.AreEqual(typeof(IDisposable).Name, e.TypeRequested);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        [TestMethod]
        public void DuplicateTypeMappingExceptionIsSerializedAcrossAppDomainBoundaries()
        {
            var domain =
                AppDomain.CreateDomain("test", null, new AppDomainSetup { ApplicationBase = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) });

            try
            {
                domain.DoCallBack(() =>
                {
                    throw new DuplicateTypeMappingException("some name", typeof(IDisposable), typeof(object), typeof(string));
                });
            }
            catch (DuplicateTypeMappingException e)
            {
                Assert.IsNotNull(e.Message);
                Assert.AreEqual("some name", e.Name);
                Assert.AreEqual(typeof(IDisposable).AssemblyQualifiedName, e.MappedFromType);
                Assert.AreEqual(typeof(object).AssemblyQualifiedName, e.CurrentMappedToType);
                Assert.AreEqual(typeof(string).AssemblyQualifiedName, e.NewMappedToType);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }
    }
}
