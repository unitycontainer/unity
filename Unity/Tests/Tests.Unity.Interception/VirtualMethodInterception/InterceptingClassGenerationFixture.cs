using System;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

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
            InterceptingClassGenerator generator = new InterceptingClassGenerator(baseType,
                interceptor.GetInterceptableMethods(baseType, baseType));
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
        public void CanAddHandlersToPipeline()
        {
            MethodInfo methodOne = typeof (ClassWithDefaultCtor).GetMethod("MethodOne");
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            IInterceptingProxy pm = (IInterceptingProxy) instance;

            CallCountHandler handler = new CallCountHandler();

            HandlerPipeline pipeline = new HandlerPipeline(new CallCountHandler[] { handler });
            pm.SetPipeline(methodOne, pipeline);
        }

        [TestMethod]
        public void CallingMethodInvokesHandlers()
        {
            MethodInfo methodOne = typeof (ClassWithDefaultCtor).GetMethod("MethodOne");
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            IInterceptingProxy pm = (IInterceptingProxy) instance;

            CallCountHandler handler = new CallCountHandler();
            PostCallCountHandler postHandler = new PostCallCountHandler();
            HandlerPipeline pipeline = new HandlerPipeline(new ICallHandler[] { postHandler, handler });
            pm.SetPipeline(methodOne, pipeline);

            instance.MethodOne();

            Assert.AreEqual(1, handler.CallCount);
            Assert.AreEqual(1, postHandler.CallsCompleted);
        }

        [TestMethod]
        public void ThrowingFromInterceptedMethodStillRunsAllHandlers()
        {
            MethodInfo thrower = typeof (ClassWithDefaultCtor).GetMethod("NotImplemented");
            ClassWithDefaultCtor instance = WireupHelper.GetInterceptingInstance<ClassWithDefaultCtor>();
            IInterceptingProxy pm = (IInterceptingProxy) instance;

            CallCountHandler handler = new CallCountHandler();
            PostCallCountHandler postHandler = new PostCallCountHandler();
            HandlerPipeline pipeline = new HandlerPipeline(new ICallHandler[] { postHandler, handler });
            pm.SetPipeline(thrower, pipeline);

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
            Assert.AreEqual(5*2, timesTwo);
            Assert.AreEqual(1, handler.CallsCompleted);
            
        }

        [TestMethod]
        public void CanInterceptNestedClass()
        {
            PostCallCountHandler handler = new PostCallCountHandler();
            NestedClass instance = WireupHelper.GetInterceptedInstance<NestedClass>("MakeAValue", handler);

            int result = instance.MakeAValue(12);

            Assert.AreEqual(12 * 37 + 12 /2, result);
            Assert.AreEqual(1, handler.CallsCompleted);
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

            IInterceptingProxy pm = (IInterceptingProxy) instance;
            MethodBase reverse = typeof (InterceptingGenericClass<DateTime>).GetMethod("Reverse");
            pm.SetPipeline(reverse, new HandlerPipeline(Seq.Collect<ICallHandler>(handler)));

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
                return source*37 + source/2;
            }
        }

        internal class CantTouchThis
        {
            public virtual void Something()
            {
                
            }
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
            return x + (int) f;
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
        private static readonly MethodBase methodOne = typeof (ClassWithDefaultCtor).GetMethod("MethodOne");
        private static readonly MethodBase calculateAnswer = typeof (ClassWithDefaultCtor).GetMethod("CalculateAnswer");
        private static readonly MethodBase addUp = typeof (ClassWithDefaultCtor).GetMethod("AddUp");
        private static readonly MethodBase methodWithRefParameters = typeof (ClassWithDefaultCtor).GetMethod("MethodWithRefParameters");

        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">Method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        HandlerPipeline IInterceptingProxy.GetPipeline(MethodBase method)
        {
            return pipelines.GetPipeline(method.MetadataToken);
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">Method to apply the pipeline to.</param>
        /// <param name="pipeline">The new pipeline.</param>
        void IInterceptingProxy.SetPipeline(MethodBase method, HandlerPipeline pipeline)
        {
            pipelines.SetPipeline(method.MetadataToken, pipeline);
        }

        public override void MethodOne()
        {
            HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(methodOne);

            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, methodOne);
            IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                try
                {
                    BaseMethodOne();
                    return input.CreateMethodReturn(null, input.Arguments);
                }
                catch (Exception ex)
                {
                    return input.CreateExceptionMethodReturn(ex);
                }
            });

            if(result.Exception != null)
            {
                throw result.Exception;
            }
        }

        private void BaseMethodOne()
        {
            base.MethodOne();
        }

        public override int CalculateAnswer()
        {
            HandlerPipeline pipeline = ((IInterceptingProxy) this).GetPipeline(calculateAnswer);
            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, calculateAnswer);
            IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                try
                {
                    int returnValue = BaseCalculateAnswer();
                    return input.CreateMethodReturn(returnValue, input.Arguments);
                }
                catch (Exception ex)
                {
                    return input.CreateExceptionMethodReturn(ex);
                }
            });

            if(result.Exception != null)
            {
                throw result.Exception;
            }

            return (int) result.ReturnValue;
        }

        private int BaseCalculateAnswer()
        {
            return base.CalculateAnswer();
        }

        public override string AddUp(int x, int y)
        {
            HandlerPipeline pipeline = ((IInterceptingProxy) this).GetPipeline(addUp);
            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, addUp, x, y);
            IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                try
                {
                    string returnValue = BaseAddUp((int) input.Inputs[0], (int) input.Inputs[1]);
                    return input.CreateMethodReturn(returnValue, input.Inputs[0], input.Inputs[1]);
                }
                catch (Exception ex)
                {
                    return input.CreateExceptionMethodReturn(ex);
                }
            });

            if(result.Exception != null)
            {
                throw result.Exception;
            }
            return (string) result.ReturnValue;
        }

        private string BaseAddUp(int x, int y)
        {
            return base.AddUp(x, y);
        }

        public override int MethodWithRefParameters(int x, ref string y, float f)
        {
            HandlerPipeline pipeline = ((IInterceptingProxy) this).GetPipeline(methodWithRefParameters);
            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, methodWithRefParameters, x, y, f);
            IMethodReturn result = pipeline.Invoke(inputs, delegate(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                try
                {
                    string refParam = (string)input.Arguments[1];
                    int returnValue = BaseMethodWithRefParameters((int) inputs.Arguments[0], ref refParam, (float) inputs.Arguments[2]);
                    return input.CreateMethodReturn(returnValue, input.Inputs[0], refParam, input.Inputs[2]);
                }
                catch (Exception ex)
                {
                    return input.CreateExceptionMethodReturn(ex);
                }
            });

            if(result.Exception != null)
            {
                throw result.Exception;
            }
            return (int) result.ReturnValue;
        }

        private int BaseMethodWithRefParameters(int x, ref string y, float f)
        {
            return base.MethodWithRefParameters(x, ref y, f);
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
            return Sequence.ToString(chars, "");
        }
    }

    public class WrapperGeneric<T> : InterceptingGenericClass<T>, IInterceptingProxy
    {
        private PipelineManager pipelines = new PipelineManager();
        private MethodBase reverse = typeof (InterceptingGenericClass<T>).GetMethod("Reverse");

        HandlerPipeline IInterceptingProxy.GetPipeline(MethodBase method)
        {
            return pipelines.GetPipeline(method.MetadataToken);
        }

        void IInterceptingProxy.SetPipeline(MethodBase method, HandlerPipeline pipeline)
        {
            pipelines.SetPipeline(method.MetadataToken, pipeline);
        }

        private string BaseReverse<TITem>(TITem obj)
        {
            return base.Reverse(obj);
        }

        private IMethodReturn Reverse_DelegateImpl<TItem>(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            try
            {
                string baseResult = BaseReverse((TItem) input.Arguments[0]);
                return input.CreateMethodReturn(baseResult);

            }
            catch (Exception ex)
            {
                return input.CreateExceptionMethodReturn(ex);
            }
        }

        public override string Reverse<TItem>(TItem obj)
        {
            HandlerPipeline pipeline = ((IInterceptingProxy) this).GetPipeline(reverse);
            VirtualMethodInvocation inputs = new VirtualMethodInvocation(this, reverse, obj);
            IMethodReturn result = pipeline.Invoke(inputs, Reverse_DelegateImpl<TItem>);
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            return (string)result.ReturnValue;
        }
    }
}
