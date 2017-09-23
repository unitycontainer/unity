// TODO: Verify
//// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//using System;
//using System.Collections.Generic;

//#if NETFX_CORE
//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
//#elif WINDOWS_PHONE
//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
//#else
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//#endif

//namespace Unity.Tests.Override
//{
//    [TestClass]
//    public class TypeBasedOverrideConfigurationFixture
//    {
//        [TestMethod]
//        [DeploymentItem(@"ConfigFiles\TypeOverride.config", ConfigurationFixtureBase.ConfigFilesFolder)]
//        public void TypeBasedOverrideWithConstructorDefaultFromConfig()
//        {
//            ParameterOverride overrideParam = new ParameterOverride("value", 222);
//            TypeBasedOverride overrideDecorator = new TypeBasedOverride(typeof(TypeToInject2ForTypeOverride), overrideParam);

//            IUnityContainer container = ConfigurationFixtureBase.GetContainer(@"ConfigFiles\TypeOverride.config", "TypeOverrideContainer");

//            var defaultResult = container.Resolve<TypeToUndergoeTypeBasedInject1>("TestTypeOverrideDefaultInConfiguration");
//            var overrideResult = container.Resolve<TypeToUndergoeTypeBasedInject1>("TestTypeOverrideDefaultInConfiguration", overrideDecorator);

//            Assert.AreEqual<int>(101, defaultResult.IForTypeToInject.Value);
//            Assert.AreEqual<int>(222, overrideResult.IForTypeToInject.Value);
//        }
//    }
//}
