using System;
using System.Collections.Generic;
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
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            Assert.AreNotSame(typeof(ClassWithDefaultCtor), instance.GetType());
        }

        [TestMethod]
        public void InterceptingClassCallsBaseClassConstructor()
        {
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            Assert.IsTrue(instance.CtorWasCalled);
        }

        [TestMethod]
        public void CanCreateInterceptingClassOverClassWithoutDefaultConstructor()
        {
            ClassWithOneParamCtor instance = GetInterceptingInstance<ClassWithOneParamCtor>(37);
            Assert.AreEqual(37, instance.CtorValue);
        }

        [TestMethod]
        public void CanInterceptClassThatHasMultipleConstructors()
        {
            ClassWithMultipleCtors defaultInstance = GetInterceptingInstance<ClassWithMultipleCtors>();
            Assert.IsTrue(defaultInstance.DefaultCalled);

            ClassWithMultipleCtors intInstance = GetInterceptingInstance<ClassWithMultipleCtors>(42);
            Assert.AreEqual(42, intInstance.IntValue);
            Assert.IsFalse(intInstance.DefaultCalled);

            ClassWithMultipleCtors bothInstance = GetInterceptingInstance<ClassWithMultipleCtors>(51, "Hello");
            Assert.AreEqual(51, bothInstance.IntValue);
            Assert.AreEqual("Hello", bothInstance.StringValue);
            Assert.IsFalse(bothInstance.DefaultCalled);
        }

        [TestMethod]
        public void CanInterceptVoidNoArgMethods()
        {
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            instance.MethodOne();
            Assert.IsTrue(instance.OneWasCalled);
        }

        [TestMethod]
        public void InterceptingClassOverridesBaseClassVirtualMethods()
        {
            Type baseType = typeof(ClassWithDefaultCtor);
            InterceptingClassGenerator generator = new InterceptingClassGenerator(baseType);
            Type generatedType = generator.GenerateType();

            MethodInfo methodOne = generatedType.GetMethod("MethodOne");
            MethodInfo calculateAnswer = generatedType.GetMethod("CalculateAnswer");

            Assert.AreSame(generatedType, methodOne.DeclaringType);
            Assert.AreSame(generatedType, calculateAnswer.DeclaringType);
        }

        [TestMethod]
        public void InterceptingClassImplementsIHandlerPipelineManager()
        {
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            Assert.IsTrue(instance is IHandlerPipelineManager);
        }

        [TestMethod]
        public void CanAddHandlersToPipeline()
        {
            MethodInfo methodOne = typeof (ClassWithDefaultCtor).GetMethod("MethodOne");
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            IHandlerPipelineManager pm = (IHandlerPipelineManager) instance;

            CallCountHandler handler = new CallCountHandler();

            HandlerPipeline pipeline = new HandlerPipeline(new CallCountHandler[] { handler });
            pm.SetPipeline(methodOne.MetadataToken, pipeline);
        }

        [TestMethod]
        public void CallingMethodInvokesHandlers()
        {
            MethodInfo methodOne = typeof (ClassWithDefaultCtor).GetMethod("MethodOne");
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            IHandlerPipelineManager pm = (IHandlerPipelineManager) instance;

            CallCountHandler handler = new CallCountHandler();
            PostCallCountHandler postHandler = new PostCallCountHandler();
            HandlerPipeline pipeline = new HandlerPipeline(new ICallHandler[] { postHandler, handler });
            pm.SetPipeline(methodOne.MetadataToken, pipeline);

            instance.MethodOne();

            Assert.AreEqual(1, handler.CallCount);
            Assert.AreEqual(1, postHandler.CallsCompleted);
        }

        [TestMethod]
        public void ThrowingFromInterceptedMethodStillRunsAllHandlers()
        {
            MethodInfo thrower = typeof (ClassWithDefaultCtor).GetMethod("NotImplemented");
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            IHandlerPipelineManager pm = (IHandlerPipelineManager) instance;

            CallCountHandler handler = new CallCountHandler();
            PostCallCountHandler postHandler = new PostCallCountHandler();
            HandlerPipeline pipeline = new HandlerPipeline(new ICallHandler[] { postHandler, handler });
            pm.SetPipeline(thrower.MetadataToken, pipeline);

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
            MethodInfo getAnswer = typeof (ClassWithDefaultCtor).GetMethod("CalculateAnswer");
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            IHandlerPipelineManager pm = (IHandlerPipelineManager) instance;

            PostCallCountHandler handler = new PostCallCountHandler();
            pm.SetPipeline(getAnswer.MetadataToken,
                new HandlerPipeline(Seq.Collect<ICallHandler>(handler)));
            
            int result = instance.CalculateAnswer();

            Assert.AreEqual(42, result);
            Assert.AreEqual(1, handler.CallsCompleted);

        }

        [TestMethod]
        public void CanInterceptMethodsThatReturnReferenceTypes()
        {
            MethodInfo getAnswer = typeof (ClassWithDefaultCtor).GetMethod("GimmeName");
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            IHandlerPipelineManager pm = (IHandlerPipelineManager) instance;

            PostCallCountHandler handler = new PostCallCountHandler();
            pm.SetPipeline(getAnswer.MetadataToken,
                new HandlerPipeline(Seq.Collect<ICallHandler>(handler)));

            string result = instance.GimmeName();

            Assert.AreEqual("name", result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        [TestMethod]
        public void CanInterceptMethodsWithParameters()
        {
            MethodInfo addUp = typeof (ClassWithDefaultCtor).GetMethod("AddUp");
            ClassWithDefaultCtor instance = GetInterceptingInstance<ClassWithDefaultCtor>();
            IHandlerPipelineManager pm = (IHandlerPipelineManager) instance;

            PostCallCountHandler handler = new PostCallCountHandler();
            pm.SetPipeline(addUp.MetadataToken,
                new HandlerPipeline(Seq.Collect<ICallHandler>(handler)));

            string result = instance.AddUp(5, 12);

            Assert.AreEqual("17", result);
            Assert.AreEqual(1, handler.CallsCompleted);
        }

        private static T GetInterceptingInstance<T>(params object[] ctorValues)
        {
            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeof(T));
            Type generatedType = generator.GenerateType();

            return (T)Activator.CreateInstance(generatedType, ctorValues);
        }
    }

    public class ClassWithDefaultCtor
    {
        public bool CtorWasCalled = false;
        public bool OneWasCalled = false;

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
    }

    public class Wrapper : ClassWithDefaultCtor, IHandlerPipelineManager
    {
        private readonly PipelineManager pipelines = new PipelineManager();
        private static readonly MethodBase methodOne = typeof (ClassWithDefaultCtor).GetMethod("MethodOne");
        private static readonly MethodBase calculateAnswer = typeof (ClassWithDefaultCtor).GetMethod("CalculateAnswer");
        private static readonly MethodBase addUp = typeof (ClassWithDefaultCtor).GetMethod("AddUp");
        /// <summary>
        /// Retrieve the pipeline assocated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">Method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        HandlerPipeline IHandlerPipelineManager.GetPipeline(int method)
        {
            return pipelines.GetPipeline(method);
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">Method to apply the pipeline to.</param>
        /// <param name="pipeline">The new pipeline.</param>
        void IHandlerPipelineManager.SetPipeline(int method, HandlerPipeline pipeline)
        {
            pipelines.SetPipeline(method, pipeline);
        }


        public override void MethodOne()
        {
            HandlerPipeline pipeline = ((IHandlerPipelineManager)this).GetPipeline(methodOne.MetadataToken);

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
            HandlerPipeline pipeline = ((IHandlerPipelineManager) this).GetPipeline(calculateAnswer.MetadataToken);
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
            HandlerPipeline pipeline = ((IHandlerPipelineManager) this).GetPipeline(addUp.MetadataToken);
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
    }

    public class ClassWithOneParamCtor
    {
        public int CtorValue = 0;

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




}
