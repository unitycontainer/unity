// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using ObjectBuilder2;
using Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Unity.TestSupport;
using Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    [TestClass]
    public partial class InterceptingClassGenerationFixture
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

            Assert.AreEqual((12 * 37) + (12 / 2), result);
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
                return (source * 37) + (source / 2);
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
            ((InterfaceImplementingMainType)instance).DoSomething(String.Empty);
            ((IDoSomething)instance).DoSomething();
            ((IDoSomething)instance).DoSomething(String.Empty);
            ((IDeriveFromIDoSomething)instance).DoSomething();
            ((IDeriveFromIDoSomething)instance).DoSomething(String.Empty);
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

    // This class isn't actually used, it was a template we used to decompile the
    // IL and figure out what code to generate.
    public class Wrapper : ClassWithDefaultCtor, IInterceptingProxy
    {
        private readonly InterceptionBehaviorPipeline pipeline = new InterceptionBehaviorPipeline();

        private static readonly MethodBase MethodOneMethod = typeof(ClassWithDefaultCtor).GetMethod("MethodOne");
        private static readonly MethodBase CalculateAnswerMethod = typeof(ClassWithDefaultCtor).GetMethod("CalculateAnswer");
        private static readonly MethodBase AddUpMethod = typeof(ClassWithDefaultCtor).GetMethod("AddUp");
        private static readonly MethodBase MethodWithRefParametersMethod = typeof(ClassWithDefaultCtor).GetMethod("MethodWithRefParameters");
        private static readonly MethodBase OutParamsMethod = typeof(ClassWithDefaultCtor).GetMethod("OutParams");

        private static readonly MethodBase MethodWithGenericReturnTypeMethod = typeof(ClassWithDefaultCtor).GetMethods()
            .Where(m => m.Name == "MethodWithGenericReturnType").First();

        public override T MethodWithGenericReturnType<T>(T item)
        {
            var input = new VirtualMethodInvocation(this, Wrapper.MethodWithGenericReturnTypeMethod, item);
            IMethodReturn result = this.pipeline.Invoke(input, this.MethodWithGenericReturnType_Delegate<T>);
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            return (T)result.ReturnValue;
        }

        private IMethodReturn MethodWithGenericReturnType_Delegate<T>(IMethodInvocation inputs, GetNextInterceptionBehaviorDelegate getNext)
        {
            try
            {
                T result = base.MethodWithGenericReturnType((T)inputs.Arguments[0]);
                return inputs.CreateMethodReturn(result, inputs.Arguments);
            }
            catch (Exception ex)
            {
                return inputs.CreateExceptionMethodReturn(ex);
            }
        }

        /// <summary>
        /// Adds a <see cref="IInterceptionBehavior"/> to the proxy.
        /// </summary>
        /// <param name="interceptor">The <see cref="IInterceptionBehavior"/> to add.</param>
        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            this.pipeline.Add(interceptor);
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
            return chars.JoinStrings(String.Empty);
        }
    }

    public class WrapperGeneric<T> : InterceptingGenericClass<T>, IInterceptingProxy
    {
        private InterceptionBehaviorPipeline pipeline = new InterceptionBehaviorPipeline();
        private MethodBase reverse = typeof(InterceptingGenericClass<T>).GetMethod("Reverse");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists", Justification = "Point of the test is to call base class and Reverse is overridden and virtual.")]
        private string BaseReverse<TITem>(TITem obj)
        {
            return base.Reverse(obj);
        }

        public void AddInterceptionBehavior(IInterceptionBehavior interceptor)
        {
            this.pipeline.Add(interceptor);
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
            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, this.reverse, obj);
            IMethodReturn result = this.pipeline.Invoke(inputs, this.Reverse_DelegateImpl<TItem>);
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
        private int propertyValue;
        
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
        private int propertyValue;
        
        public virtual int Property
        {
            get { return propertyValue; }
            protected set { propertyValue = value; }
        }
    }

#if !SILVERLIGHT
    [System.Web.AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal)]
#endif
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
