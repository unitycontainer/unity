// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests
{
    /// <summary>
    /// An implementation of IMethodCallMessage that can be used to mock
    /// out calls.
    /// </summary>
    internal class FakeMethodCallMessage : IMethodCallMessage
    {
        private MethodBase methodBase;
        private object[] arguments;
        private List<int> inputArgumentIndex = new List<int>();
        private List<int> outputArgumentIndex = new List<int>();
        private ParameterInfo[] parameterInfo;

        private Hashtable properties = new Hashtable();

        public FakeMethodCallMessage(MethodBase methodBase, params object[] arguments)
        {
            this.methodBase = methodBase;
            this.parameterInfo = methodBase.GetParameters();

            this.arguments = arguments;
            int i = 0;
            foreach (ParameterInfo param in parameterInfo)
            {
                if (param.IsOut)
                {
                    outputArgumentIndex.Add(i);
                }
                else
                {
                    inputArgumentIndex.Add(i);
                }
                ++i;
            }
        }

        public string GetInArgName(int index)
        {
            return parameterInfo[inputArgumentIndex[index]].Name;
        }

        public object GetInArg(int argNum)
        {
            return arguments[inputArgumentIndex[argNum]];
        }

        public int InArgCount
        {
            get { return inputArgumentIndex.Count; }
        }

        public object[] InArgs
        {
            get
            {
                object[] inArgs = new object[InArgCount];
                int inArgsIndex = 0;
                inputArgumentIndex.ForEach(delegate(int i)
                                           {
                                               inArgs[inArgsIndex++] = arguments[i];
                                           });
                return inArgs;
            }
        }

        public string GetArgName(int index)
        {
            return parameterInfo[index].Name;
        }

        public object GetArg(int argNum)
        {
            return arguments[argNum];
        }

        public string Uri
        {
            get { return "http://tempuri.org/FakeMethodCallMessage"; }
        }

        public string MethodName
        {
            get { return methodBase.Name; }
        }

        public string TypeName
        {
            get { return methodBase.DeclaringType.Name; }
        }

        public object MethodSignature
        {
            get
            {
                List<Type> signatureTypes = new List<Type>();
                foreach (ParameterInfo paramInfo in parameterInfo)
                {
                    signatureTypes.Add(paramInfo.ParameterType);
                }
                return signatureTypes.ToArray();
            }
        }

        public int ArgCount
        {
            get { return parameterInfo.Length; }
        }

        public object[] Args
        {
            get { return arguments; }
        }

        public bool HasVarArgs
        {
            get { return false; }
        }

        public LogicalCallContext LogicalCallContext
        {
            get { return null; }
        }

        public MethodBase MethodBase
        {
            get { return methodBase; }
        }

        public IDictionary Properties
        {
            get { return properties; }
        }
    }
}
