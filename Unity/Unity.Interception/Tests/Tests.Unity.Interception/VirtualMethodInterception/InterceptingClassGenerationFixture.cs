//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    [TestClass]
    public class InterceptingClassGenerationFixture
    {
        [TestMethod]
        public void CanCreateInterceptingClassOverClassWithDefaultConstructor()
        {
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            Assert.AreNotSame(typeof(ClassWithDefaultCtor), instance.GetType());
        }

        [TestMethod]
        public void InterceptingClassCallsBaseClassConstructor()
        {
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            Assert.IsTrue(instance.CtorWasCalled);
        }

        [TestMethod]
        public void CanCreateInterceptingClassOverClassWithoutDefaultConstructor()
        {
            ClassWithOneParamCtor instance = WireupHelper.GetInterceptingInstance<ClassWithOneParamCtor>(37);
            Assert.AreEqual(37, instance.CtorValue);
        }

        [TestMethod]
        public void CanInterceptClassThatHasMultipleConstructors()
        {
            ClassWithMultipleCtors defaultInstance = WireupHelper.GetInterceptingInstance<ClassWithMultipleCtors>();
            Assert.IsTrue(defaultInstance.DefaultCalled);

            ClassWithMultipleCtors intInstance = WireupHelper.GetInterceptingInstance<ClassWithMultipleCtors>(42);
            Assert.AreEqual(42, intInstance.IntValue);
            Assert.IsFalse(intInstance.DefaultCalled);

            ClassWithMultipleCtors bothInstance = WireupHelper.GetInterceptingInstance<ClassWithMultipleCtors>(51, "Hello");
            Assert.AreEqual(51, bothInstance.IntValue);
            Assert.AreEqual("Hello", bothInstance.StringValue);
            Assert.IsFalse(bothInstance.DefaultCalled);
        }

        [TestMethod]
        public void CanInterceptVoidNoArgMethods()
        {
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            instance.MethodOne();
            Assert.IsTrue(instance.OneWasCalled);
        }

        [TestMethod]
        public void InterceptingClassOverridesBaseClassVirtualMethods()
        {
            Type baseType = typeof(ClassWithDefaultCtor);
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            InterceptingClassGenerator generator = new InterceptingClassGenerator(baseType);
            Type generatedType = generator.GenerateType();

            MethodInfo methodOne = generatedType.GetMethod("MethodOne");
            MethodInfo calculateAnswer = generatedType.GetMethod("CalculateAnswer");

            Assert.AreSame(generatedType, methodOne.DeclaringType);
            Assert.AreSame(generatedType, calculateAnswer.DeclaringType);
        }

        [TestMethod]
        public void InterceptingClassImplementsIInterceptingProxy()
        {
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            Assert.IsTrue(instance is IInterceptingProxy);
        }

        [TestMethod]
        public void CanAddInterceptionBehaviorsToPipeline()
        {
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            IInterceptingProxy pm = (IInterceptingProxy)instance;

            CallCountInterceptionBehavior interceptor = new CallCountInterceptionBehavior();

            pm.AddInterceptionBehavior(interceptor);
        }

        [TestMethod]
        public void CallingMethodInvokesHandlers()
        {
            MethodInfo methodOne = typeof(ClassWithDefaultCtor).GetMethod("MethodOne");
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            IInterceptingProxy pm = (IInterceptingProxy)instance;

            CallCountHandler handler = new CallCountHandler();
            PostCallCountHandler postHandler = new PostCallCountHandler();
            HandlerPipeline pipeline = new HandlerPipeline(new ICallHandler[] { postHandler, handler });
            PipelineManager manager = new PipelineManager();
            manager.SetPipeline(methodOne, pipeline);
            pm.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));

            instance.MethodOne();

            Assert.AreEqual(1, handler.CallCount);
            Assert.AreEqual(1, postHandler.CallsCompleted);
        }

        [TestMethod]
        public void ThrowingFromInterceptedMethodStillRunsAllHandlers()
        {
            MethodInfo thrower = typeof(ClassWithDefaultCtor).GetMethod("NotImplemented");
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            IInterceptingProxy pm = (IInterceptingProxy)instance;

            CallCountHandler handler = new CallCountHandler();
            PostCallCountHandler postHandler = new PostCallCountHandler();
            HandlerPipeline pipeline = new HandlerPipeline(new ICallHandler[] { postHandler, handler });
            PipelineManager manager = new PipelineManager();
            manager.SetPipeline(thrower, pipeline);
            pm.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));

            try
            {
                instance.NotImplemented();
                Assert.Fail("Should have thrown before getting here");
            }
            catch (NotImplementedException)
            {
                // We're expecting this one
            }

            Assert.AreEqual(1, handler.CallCount);
            Assert.AreEqual(1, postHandler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptMethodsThatHaveReturnValues()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptedInstance<ClassWithDefaultCtor>("CalculateAnswer", handler);

            int result = instance.CalculateAnswer();

            Assert.AreEqual(42, result);
            Assert.AreEqual(1, handler.CallsCompleted);

        }

        [TestMethod]
        public void CanInterceptMethodsThatReturnReferenceTypes()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptedInstance<ClassWithDefaultCtor>("GimmeName", handler);

            string result = instance.GimmeName();

            Assert.AreEqual("name", result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptMethodsWithParameters()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptedInstance<ClassWithDefaultCtor>("AddUp", handler);

            string result = instance.AddUp(5, 12);

            Assert.AreEqual("17", result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptMethodsWithRefParameters()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptedInstance<ClassWithDefaultCtor>("MethodWithRefParameters", handler);

            string s = "abc";
            int result = instance.MethodWithRefParameters(5, ref s, 10);

            Assert.AreEqual(15, result);
            Assert.AreEqual("abc hooray!", s);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptMethodsWithOutParameters()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptedInstance<ClassWithDefaultCtor>("OutParams", handler);

            int plusOne;
            int timesTwo;

            instance.OutParams(5, out plusOne, out timesTwo);

            Assert.AreEqual(5 + 1, plusOne);
            Assert.AreEqual(5 * 2, timesTwo);
            Assert.AreEqual(1, handler.CallsCompleted);

        }

        [TestMethod]
        public void CanInterceptNestedClass()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            NestedClass instance = WireupHelper.GetInterceptedInstance<NestedClass>("MakeAValue", handler);

            int result = instance.MakeAValue(12);

            Assert.AreEqual(12 * 37 + 12 / 2, result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptEventsMethods()
        {
            CallCountInterceptionBehavior interceptor = new CallCountInterceptionBehavior();
            ClassWithAVirtualEvent instance = WireupHelper.GetInterceptingInstance<ClassWithAVirtualEvent>();
            ((IInterceptingProxy)instance).AddInterceptionBehavior(interceptor);

            instance.SomeEvent += (sender, args) => { };

            Assert.AreEqual(1, interceptor.CallCount);
        }

        [TestMethod]
        public void FiringAnEventIsNotIntercepted()
        {
            CallCountInterceptionBehavior interceptor = new CallCountInterceptionBehavior();
            ClassWithAVirtualEvent instance = WireupHelper.GetInterceptingInstance<ClassWithAVirtualEvent>();
            ((IInterceptingProxy)instance).AddInterceptionBehavior(interceptor);

            instance.SomeEvent += (sender, args) => { };
            instance.TriggerIt();

            Assert.AreEqual(1, interceptor.CallCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AttemptingToInterceptInvalidClassThrows()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();

            interceptor.CreateProxyType(typeof(CantTouchThis));
        }

        [TestMethod]
        public void CanInterceptClosedGenericType()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            InterceptingGenericClass<DateTime> instance =
                WireupHelper.GetInterceptedInstance<InterceptingGenericClass<DateTime>>("Decorate", handler);

            DateTime now = DateTime.Now;

            string result = instance.Decorate(now);

            Assert.AreEqual("**" + now + "**", result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptGenericMethodOnClosedGenericType()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            InterceptingGenericClass<DateTime> instance =
                WireupHelper.GetInterceptedInstance<InterceptingGenericClass<DateTime>>("Reverse", handler);

            DateTime now = DateTime.Now;

            string result = instance.Reverse(137);

            Assert.AreEqual("731", result);
            Assert.AreEqual(1, handler.CallsCompleted);

        }

        [TestMethod]
        public void GenericWrapperWorks()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            InterceptingGenericClass<DateTime> instance =
                new WrapperGeneric<DateTime>();

            PipelineManager pm = new PipelineManager();
            MethodBase reverse = typeof(InterceptingGenericClass<DateTime>).GetMethod("Reverse");
            pm.SetPipeline(reverse, new HandlerPipeline(Sequence.Collect<ICallHandler>(handler)));
            ((IInterceptingProxy)instance).AddInterceptionBehavior(new PolicyInjectionBehavior(pm));

            DateTime now = DateTime.Now;

            string result = instance.Reverse(137);

            Assert.AreEqual("731", result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptMethodWithGenericReturnTypeForValueTypeGenericParameter()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            ClassWithDefaultCtor instance
                = WireupHelper.GetInterceptedInstance<ClassWithDefaultCtor>("MethodWithGenericReturnType", handler);
            int value = instance.MethodWithGenericReturnType(5);
            Assert.AreEqual(5, value);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        public class NestedClass
        {
            public virtual int MakeAValue(int source)
            {
                return source * 37 + source / 2;
            }
        }

        internal class CantTouchThis
        {
            public virtual void Something()
            {

            }
        }

        [TestMethod]
        public void CanImplementAdditionalInterfaces()
        {
            // arrange
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(MainType), typeof(IAdditionalInterface));
            Type generatedType = generator.GenerateType();

            // act
            object instance = Activator.CreateInstance(generatedType);

            // assert
            Assert.IsTrue(instance is MainType);
            Assert.IsTrue(instance is IAdditionalInterface);
        }

        [TestMethod]
        public void InvokingMethodOnAdditionalInterfaceThrowsIfNotHandledByInterceptor()
        {
            // arrange
            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeof(MainType), typeof(IAdditionalInterface));
            Type generatedType = generator.GenerateType();
            object instance = Activator.CreateInstance(generatedType);

            // act
            Exception exception = null;
            try
            {
                ((IAdditionalInterface)instance).DoSomethingElse();
                Assert.Fail("should have thrown");
            }
            catch (NotImplementedException e)
            {
                exception = e;
            }

            // assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void CanSuccessfullyInvokeAnAdditionalInterfaceMethodIfAnInterceptorDoesNotForwardTheCall()
        {
            // arrange
            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeof(MainType), typeof(IAdditionalInterface));
            Type generatedType = generator.GenerateType();
            object instance = Activator.CreateInstance(generatedType);
            bool invoked = false;
            ((IInterceptingProxy)instance).AddInterceptionBehavior(
                new DelegateInterceptionBehavior(
                    (input, getNext) => { invoked = true; return input.CreateMethodReturn(100); }));

            // act
            int result = ((IAdditionalInterface)instance).DoSomethingElse();

            // assert
            Assert.IsTrue(invoked);
            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void CanImplementINotifyPropertyChanged()
        {
            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeof(MainType), typeof(INotifyPropertyChanged));
            Type generatedType = generator.GenerateType();
            object instance = Activator.CreateInstance(generatedType);
            string changeProperty = null;
            PropertyChangedEventHandler handler = (sender, args) => changeProperty = args.PropertyName;

            ((IInterceptingProxy)instance).AddInterceptionBehavior(new NaiveINotifyPropertyChangedInterceptionBehavior());
            ((INotifyPropertyChanged)instance).PropertyChanged += handler;

            ((MainType)instance).IntProperty = 100;

            Assert.AreEqual(100, ((MainType)instance).IntProperty);
            Assert.AreEqual("IntProperty", changeProperty);

            changeProperty = null;
            ((INotifyPropertyChanged)instance).PropertyChanged -= handler;

            ((MainType)instance).IntProperty = 200;

            Assert.AreEqual(200, ((MainType)instance).IntProperty);
            Assert.AreEqual(null, changeProperty);
        }

        [TestMethod]
        public void CanImplementAdditionalInterfaceWithMethodsHavingSignaturesMatchingMethodsInTheBaseClass()
        {
            // arrange
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(MainType), typeof(IDoSomething));
            Type generatedType = generator.GenerateType();

            // act
            object instance = Activator.CreateInstance(generatedType);

            // assert
            Assert.IsTrue(instance is MainType);
            Assert.IsTrue(instance is IDoSomething);
        }

        [TestMethod]
        public void GeneratedTypeForAdditionalInterfaceWithMethodsHavingSignaturesMatchingMethodsInTheBaseClassIsVerifiable()
        {
            PermissionSet grantSet = new PermissionSet(PermissionState.None);
            grantSet.AddPermission(
                new SecurityPermission(
                    SecurityPermissionFlag.Execution
                    | SecurityPermissionFlag.ControlEvidence
                    | SecurityPermissionFlag.ControlPolicy));
            grantSet.AddPermission(
                new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess
                    | ReflectionPermissionFlag.ReflectionEmit
                    | ReflectionPermissionFlag.MemberAccess));
            grantSet.AddPermission(new FileIOPermission(PermissionState.Unrestricted));

            AppDomain sandbox =
                AppDomain.CreateDomain(
                    "sandbox",
                    AppDomain.CurrentDomain.Evidence,
                    new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                    grantSet);

            sandbox.DoCallBack(() =>
                {
                    InterceptingClassGenerator generator =
                        new InterceptingClassGenerator(typeof(MainType), typeof(IDoSomething), typeof(IDoSomethingToo));
                    Type generatedType = generator.GenerateType();
                });
        }

        [TestMethod]
        public void CanInvokeMethodsFromDifferentTypesWithMatchingSignatures()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(MainType), typeof(IDoSomething), typeof(IDoSomethingToo));
            Type generatedType = generator.GenerateType();

            object instance = Activator.CreateInstance(generatedType);
            List<MethodBase> invokedMethods = new List<MethodBase>();
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, n) =>
                    {
                        invokedMethods.Add(mi.MethodBase);
                        return mi.CreateMethodReturn(1);
                    }));

            ((MainType)instance).DoSomething();
            ((IDoSomething)instance).DoSomething();
            ((IDoSomethingToo)instance).DoSomething();

            Assert.AreSame(StaticReflection.GetMethodInfo<MainType>(i => i.DoSomething()), invokedMethods[0]);
            Assert.AreSame(StaticReflection.GetMethodInfo<IDoSomething>(i => i.DoSomething()), invokedMethods[1]);
            Assert.AreSame(StaticReflection.GetMethodInfo<IDoSomethingToo>(i => i.DoSomething()), invokedMethods[2]);
        }

        [TestMethod]
        public void DoesNotReImplementAdditionalInterfaceAlreadyImplementedByInterceptedClass()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(InterfaceImplementingMainType), typeof(IDeriveFromIDoSomething));
            Type generatedType = generator.GenerateType();

            object instance = Activator.CreateInstance(generatedType);
            List<MethodBase> interceptedMethods = new List<MethodBase>();
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, n) =>
                    {
                        interceptedMethods.Add(mi.MethodBase);
                        return mi.CreateMethodReturn(1);
                    }));

            ((InterfaceImplementingMainType)instance).DoSomething();
            ((InterfaceImplementingMainType)instance).DoSomething("");
            ((IDoSomething)instance).DoSomething();
            ((IDoSomething)instance).DoSomething("");
            ((IDeriveFromIDoSomething)instance).DoSomething();
            ((IDeriveFromIDoSomething)instance).DoSomething("");
            ((IDeriveFromIDoSomething)instance).DoSomethingElse();

            Assert.AreEqual(4, interceptedMethods.Count);
            // only the virtual implicit method implementation is invoked for IDoSomething
            Assert.AreSame(
                StaticReflection.GetMethodInfo<InterfaceImplementingMainType>(i => i.DoSomething(null)),
                interceptedMethods[0]);
            Assert.AreSame(
                StaticReflection.GetMethodInfo<InterfaceImplementingMainType>(i => i.DoSomething(null)),
                interceptedMethods[1]);
            Assert.AreSame(
                StaticReflection.GetMethodInfo<InterfaceImplementingMainType>(i => i.DoSomething(null)),
                interceptedMethods[2]);
            Assert.AreSame(
                StaticReflection.GetMethodInfo<IDeriveFromIDoSomething>(i => i.DoSomethingElse()),
                interceptedMethods[3]);
        }

        [TestMethod]
        public void CanInterceptProtectedVirtualProperties()
        {
            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeof(ClassWithProtectedProperty));
            Type generatedType = generator.GenerateType();

            ClassWithProtectedProperty instance = (ClassWithProtectedProperty)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) =>
                    {
                        intercepts++;
                        return gn()(mi, gn);
                    }));

            instance.SetProperty(10);
            int value = instance.GetProperty();

            Assert.AreEqual(10, value);
            Assert.AreEqual(2, intercepts);
        }

        [TestMethod]
        public void CanInterceptProtectedInternalVirtualProperties()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(ClassWithProtectedInternalProperty));
            Type generatedType = generator.GenerateType();

            ClassWithProtectedInternalProperty instance =
                (ClassWithProtectedInternalProperty)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) =>
                    {
                        intercepts++;
                        return gn()(mi, gn);
                    }));

            instance.SetProperty(10);
            int value = instance.GetProperty();

            Assert.AreEqual(10, value);
            Assert.AreEqual(2, intercepts);
        }

        [TestMethod]
        public void DoesNotInterceptInternalVirtualProperties()
        {
            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeof(ClassWithInternalProperty));
            Type generatedType = generator.GenerateType();

            ClassWithInternalProperty instance = (ClassWithInternalProperty)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) =>
                    {
                        intercepts++;
                        return gn()(mi, gn);
                    }));

            instance.SetProperty(10);
            int value = instance.GetProperty();

            Assert.AreEqual(10, value);
            Assert.AreEqual(0, intercepts);
        }

        [TestMethod]
        public void CanInterceptProtectedAccesorOnMixedPrivateProtectedVirtualProperties()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(ClassWithMixedPrivateProtectedProperty));
            Type generatedType = generator.GenerateType();

            ClassWithMixedPrivateProtectedProperty instance =
                (ClassWithMixedPrivateProtectedProperty)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) =>
                    {
                        intercepts++;
                        return gn()(mi, gn);
                    }));

            instance.SetProperty(10);
            int value = instance.GetProperty();

            Assert.AreEqual(10, value);
            Assert.AreEqual(1, intercepts);
        }

        [TestMethod]
        public void CanInterceptMixedPublicProtectedVirtualProperties()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(ClassWithMixedPublicProtectedProperty));
            Type generatedType = generator.GenerateType();

            ClassWithMixedPublicProtectedProperty instance =
                (ClassWithMixedPublicProtectedProperty)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) =>
                    {
                        intercepts++;
                        return gn()(mi, gn);
                    }));

            instance.SetProperty(10);
            int value = instance.GetProperty();

            Assert.AreEqual(10, value);
            Assert.AreEqual(2, intercepts);
        }

        [TestMethod]
        public void CanInterceptClassWithReservedTypeAttributes()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(ClassWithPermissionAttribute));
            Type generatedType = generator.GenerateType();

            ClassWithPermissionAttribute instance = (ClassWithPermissionAttribute)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) => { intercepts++; return gn()(mi, gn); }));

            instance.Method();

            Assert.AreEqual(1, intercepts);
        }

        [TestMethod]
        [Ignore]    // REVIEW should this test hold?
        public void TypeDerivedFromTypeWithReservedTypeAttributeGetsReservedValuesFromClonedCustomAttributes()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(ClassWithPermissionAttribute));
            Type generatedType = generator.GenerateType();

            Assert.AreEqual(typeof(ClassWithPermissionAttribute).Attributes, generatedType.Attributes);
        }

        [TestMethod]
        [Ignore]    // REVIEW just not possible?
        public void CanInterceptAliasedMethods()
        {
            InterceptingClassGenerator generator =
                new InterceptingClassGenerator(typeof(DerivedFromMainType));
            Type generatedType = generator.GenerateType();

            DerivedFromMainType instance = (DerivedFromMainType)Activator.CreateInstance(generatedType);
            int intercepts = 0;
            ((IInterceptingProxy)instance)
                .AddInterceptionBehavior(
                    new DelegateInterceptionBehavior((mi, gn) => { intercepts++; return gn()(mi, gn); }));

            int fromDerivedType = instance.DoSomething();
            int fromMainType = ((MainType)instance).DoSomething();

            Assert.AreEqual(2, intercepts);
            Assert.AreEqual(10, fromDerivedType);
            Assert.AreEqual(5, fromMainType);
        }
    }

    public class ClassWithDefaultCtor
    {
        public bool CtorWasCalled;
        public bool OneWasCalled;

        public ClassWithDefaultCtor()
        {
            CtorWasCalled = true;
        }

        public virtual void MethodOne()
        {
            OneWasCalled = true;
        }

        public virtual void NotImplemented()
        {
            throw new NotImplementedException("Not implemented on purpose");
        }

        public virtual int CalculateAnswer()
        {
            return 42;
        }

        public virtual string GimmeName()
        {
            return "name";
        }

        public virtual string AddUp(int x, int y)
        {
            return (x + y).ToString();
        }

        public virtual int MethodWithRefParameters(int x, ref string y, float f)
        {
            y = y + " hooray!";
            return x + (int)f;
        }

        public virtual void OutParams(int x, out int plusOne, out int timesTwo)
        {
            plusOne = x + 1;
            timesTwo = x * 2;
        }

        public virtual T MethodWithGenericReturnType<T>(T item)
        {
            return item;
        }
    }

    //
    // This class isn't actually used, it was a template we used to decompile the
    // IL and figure out what code to generate.
    //
    public class Wrapper : ClassWithDefaultCtor, IInterceptingProxy
    {
        private readonly PipelineManager pipelines = new PipelineManager();
        private static readonly MethodBase methodOne = typeof(ClassWithDefaultCtor).GetMethod("MethodOne");
        private static readonly MethodBase calculateAnswer = typeof(ClassWithDefaultCtor).GetMethod("CalculateAnswer");
        private static readonly MethodBase addUp = typeof(ClassWithDefaultCtor).GetMethod("AddUp");
        private static readonly MethodBase methodWithRefParameters = typeof(ClassWithDefaultCtor).GetMethod("MethodWithRefParameters");
        private static readonly MethodBase outParams = typeof(ClassWithDefaultCtor).GetMethod("OutParams");


        public override void MethodOne()
        {
            //HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(methodOne);

            //VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, methodOne);
            //IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            //{
            //    try
            //    {
            //        BaseMethodOne();
            //        return input.CreateMethodReturn(null, input.Arguments);
            //    }
            //    catch (Exception ex)
            //    {
            //        return input.CreateExceptionMethodReturn(ex);
            //    }
            //});

            //if (result.Exception != null)
            //{
            //    throw result.Exception;
            //}
        }

        private void BaseMethodOne()
        {
            base.MethodOne();
        }

        public override int CalculateAnswer()
        {
            //HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(calculateAnswer);
            //VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, calculateAnswer);
            //IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            //{
            //    try
            //    {
            //        int returnValue = BaseCalculateAnswer();
            //        return input.CreateMethodReturn(returnValue, input.Arguments);
            //    }
            //    catch (Exception ex)
            //    {
            //        return input.CreateExceptionMethodReturn(ex);
            //    }
            //});

            //if (result.Exception != null)
            //{
            //    throw result.Exception;
            //}

            //return (int)result.ReturnValue;
            return 0;
        }

        private int BaseCalculateAnswer()
        {
            return base.CalculateAnswer();
        }

        public override string AddUp(int x, int y)
        {
            //HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(addUp);
            //VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, addUp, x, y);
            //IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            //{
            //    try
            //    {
            //        string returnValue = BaseAddUp((int)input.Inputs[0], (int)input.Inputs[1]);
            //        return input.CreateMethodReturn(returnValue, input.Inputs[0], input.Inputs[1]);
            //    }
            //    catch (Exception ex)
            //    {
            //        return input.CreateExceptionMethodReturn(ex);
            //    }
            //});

            //if (result.Exception != null)
            //{
            //    throw result.Exception;
            //}
            //return (string)result.ReturnValue;
            return null;
        }

        private string BaseAddUp(int x, int y)
        {
            return base.AddUp(x, y);
        }

        public override int MethodWithRefParameters(int x, ref string y, float f)
        {
            //HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(methodWithRefParameters);
            //VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, methodWithRefParameters, x, y, f);
            //IMethodReturn result = pipeline.Invoke(inputs, MethodWithRefParameters_Delegate);
            //if (result.Exception != null)
            //{
            //    throw result.Exception;
            //}
            //y = (string)result.Outputs[0];
            //return (int)result.ReturnValue;
            return 0;
        }

        private IMethodReturn MethodWithRefParameters_Delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            try
            {
                string refParam = (string)input.Arguments[1];
                int returnValue = BaseMethodWithRefParameters((int)input.Arguments[0], ref refParam, (float)input.Arguments[2]);
                return input.CreateMethodReturn(returnValue, input.Inputs[0], refParam, input.Inputs[2]);
            }
            catch (Exception ex)
            {
                return input.CreateExceptionMethodReturn(ex);
            }

        }

        private int BaseMethodWithRefParameters(int x, ref string y, float f)
        {
            return base.MethodWithRefParameters(x, ref y, f);
        }

        public override void OutParams(int x, out int plusOne, out int timesTwo)
        {
            //HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(outParams);
            //VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, outParams, x, default(int), default(int));
            //IMethodReturn result = pipeline.Invoke(inputs, OutParams_Delegate);
            //if (result.Exception != null)
            //{
            //    throw result.Exception;
            //}
            //plusOne = (int)result.Outputs[0];
            //timesTwo = (int)result.Outputs[1];
            plusOne = 0;
            timesTwo = 0;
        }

        private IMethodReturn OutParams_Delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            try
            {
                int outParam1;
                int outParam2;

                BaseOutParams((int)input.Arguments[0], out outParam1, out outParam2);
                return input.CreateMethodReturn(null, input.Arguments[0], outParam1, outParam2);
            }
            catch (Exception ex)
            {
                return input.CreateExceptionMethodReturn(ex);
            }
        }

        private void BaseOutParams(int x, out int plusOne, out int timesTwo)
        {
            base.OutParams(x, out plusOne, out timesTwo);
        }

        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            throw new NotImplementedException();
        }


        public System.Collections.Generic.IEnumerable<MethodImplementationInfo> GetInterceptableMethods()
        {
            throw new NotImplementedException();
        }
    }

    public class ClassWithOneParamCtor
    {
        public int CtorValue;

        public ClassWithOneParamCtor(int i)
        {
            CtorValue = i;
        }
    }

    public class ClassWithMultipleCtors
    {
        public int IntValue;
        public string StringValue;
        public bool DefaultCalled;

        public ClassWithMultipleCtors()
        {
            DefaultCalled = true;
        }

        public ClassWithMultipleCtors(int intValue)
        {
            IntValue = intValue;
        }

        public ClassWithMultipleCtors(string stringValue)
        {
            StringValue = stringValue;
        }

        public ClassWithMultipleCtors(int intValue, string stringValue)
        {
            IntValue = intValue;
            StringValue = stringValue;
        }
    }

    public class InterceptingGenericClass<T>
    {
        public virtual string Decorate(T obj)
        {
            return "**" + obj + "**";
        }

        public virtual string Reverse<TItem>(TItem obj)
        {
            char[] chars = obj.ToString().ToCharArray();
            Array.Reverse(chars);
            return chars.JoinStrings("");
        }
    }

    public class WrapperGeneric<T> : InterceptingGenericClass<T>, IInterceptingProxy
    {
        private InterceptionBehaviorPipeline pipeline = new InterceptionBehaviorPipeline();
        private MethodBase reverse = typeof(InterceptingGenericClass<T>).GetMethod("Reverse");

        private string BaseReverse<TITem>(TITem obj)
        {
            return base.Reverse(obj);
        }

        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            pipeline.Add(interceptor);
        }

        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods()
        {
            throw new NotImplementedException();
        }

        private IMethodReturn Reverse_DelegateImpl<TItem>(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            try
            {
                string baseResult = BaseReverse((TItem)input.Arguments[0]);
                return input.CreateMethodReturn(baseResult);

            }
            catch (Exception ex)
            {
                return input.CreateExceptionMethodReturn(ex);
            }
        }

        public override string Reverse<TItem>(TItem obj)
        {
            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, reverse, obj);
            IMethodReturn result = pipeline.Invoke(inputs, Reverse_DelegateImpl<TItem>);
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            return (string)result.ReturnValue;
        }
    }

    public class ClassWithAVirtualEvent
    {
        public virtual event EventHandler<EventArgs> SomeEvent;

        public void TriggerIt()
        {
            SomeEvent(this, new EventArgs());
        }
    }

    public class WrapperWithEvent : ClassWithAVirtualEvent, IInterceptingProxy
    {
        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods()
        {
            throw new NotImplementedException();
        }

        public override event EventHandler<EventArgs> SomeEvent
        {
            add
            {
                base.SomeEvent += value;
            }

            remove
            {
                base.SomeEvent -= value;
            }
        }
    }

    public class MainType
    {
        public virtual int DoSomething()
        {
            return 5;
        }

        public virtual int IntProperty { get; set; }
    }

    public interface IAdditionalInterface
    {
        int DoSomethingElse();

        string SomeProperty { get; set; }
    }

    public interface IDoSomething
    {
        int DoSomething();
        int DoSomething(string parameter);
    }

    public interface IDoSomethingToo
    {
        int DoSomething();
    }

    public class InterfaceImplementingMainType : IDoSomething
    {
        public int DoSomething()
        {
            return 1;
        }

        public virtual int DoSomething(string parameter)
        {
            return 1;
        }
    }

    public interface IDeriveFromIDoSomething : IDoSomething
    {
        int DoSomethingElse();
    }

    public class ClassWithProtectedProperty
    {
        public int GetProperty() { return Property; }
        public void SetProperty(int value) { Property = value; }
        protected virtual int Property { get; set; }
    }

    public class ClassWithInternalProperty
    {
        public int GetProperty() { return Property; }
        public void SetProperty(int value) { Property = value; }
        internal virtual int Property { get; set; }
    }

    public class ClassWithProtectedInternalProperty
    {
        public int GetProperty() { return Property; }
        public void SetProperty(int value) { Property = value; }
        protected internal virtual int Property { get; set; }
    }

    public class ClassWithMixedPrivateProtectedProperty
    {
        public int GetProperty() { return Property; }
        public void SetProperty(int value) { Property = value; }
        int propertyValue;
        protected virtual int Property
        {
            private get { return propertyValue; }
            set { propertyValue = value; }
        }
    }

    public class ClassWithMixedPublicProtectedProperty
    {
        public int GetProperty() { return Property; }
        public void SetProperty(int value) { Property = value; }
        int propertyValue;
        public virtual int Property
        {
            get { return propertyValue; }
            protected set { propertyValue = value; }
        }
    }

    [System.Web.AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal)]
    public class ClassWithPermissionAttribute
    {
        public virtual void Method()
        {
        }
    }

    public class DerivedFromMainType : MainType
    {
        public virtual new int DoSomething()
        {
            return 10;
        }
    }
}
