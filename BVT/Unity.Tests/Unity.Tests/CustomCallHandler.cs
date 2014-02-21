// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Tests
{
    public class CustomCallHandlerUsingInputAndMethodReturn : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            if (input.Inputs.Count == 4)
            {
                if (input.Inputs.ParameterName(0) != "inputParam1" || input.Inputs.ParameterName(1) != "inputParam2" || input.Inputs.ParameterName(2) != "refParam1" || input.Inputs.ParameterName(3) != "refParam2")
                {
                    throw new InvalidOperationException("input Parameter missmatch error)");
                }

                if ((string)input.Inputs[0] != "inputParam1" || (string)input.Inputs[1] != "inputParam2" || (string)input.Inputs[2] != "refParam1" || (string)input.Inputs[3] != "refParam2")
                {
                    throw new InvalidOperationException("input Parameter missmatch error)");
                }
            }

            IMethodReturn returnValue = getNext().Invoke(input, getNext);
            if (returnValue.Outputs.Count == 4)
            {
                if (returnValue.Outputs.ParameterName(0) != "outParam1" || returnValue.Outputs.ParameterName(1) != "outParam2" || returnValue.Outputs.ParameterName(2) != "refParam1" || returnValue.Outputs.ParameterName(3) != "refParam2")
                {
                    throw new InvalidOperationException("Parameter ordetring missmatch in output collection)");
                }

                if ((string)returnValue.Outputs[0] != "inside target method outparam1" || (string)returnValue.Outputs[1] != "inside target method outparam2")
                {
                    throw new InvalidOperationException("Out Parameter values not reflected correctly in returnvalue outputs collection)");
                }

                if ((string)returnValue.Outputs[2] != "refParam1inside target method refParam1" || (string)returnValue.Outputs[3] != "refParam2inside target method refParam2")
                {
                    string test1 = (string)returnValue.Outputs[2];
                    string test2 = (string)returnValue.Outputs[3];

                    throw new InvalidOperationException("Out Parameter values not reflected correctly in returnvalue outputs collection)");
                }
            }
            return returnValue;
        }

        public int Order { get; set; }
    }

    public class CustomCallHandlerUsingArguments : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            if (input.Inputs.Count == 4)
            {
                if (input.Inputs.ParameterName(0) != "inputParam1" || input.Inputs.ParameterName(1) != "inputParam2" || input.Inputs.ParameterName(2) != "refParam1" || input.Inputs.ParameterName(3) != "refParam2")
                {
                    throw new InvalidOperationException("input Parameter missmatch error)");
                }
         
                if ((string)input.Inputs[0] != "inputParam1" || (string)input.Inputs[1] != "inputParam2" || (string)input.Inputs[2] != "refParam1" || (string)input.Inputs[3] != "refParam2")
                {
                    throw new InvalidOperationException("input Parameter missmatch error)");
                }
            }

            if (input.Arguments.Count == 6)
            {
                if (input.Arguments.ParameterName(0) != "inputParam1" || input.Arguments.ParameterName(1) != "inputParam2" || input.Arguments.ParameterName(2) != "outParam1" || input.Arguments.ParameterName(3) != "outParam2" || input.Arguments.ParameterName(4) != "refParam1" || input.Arguments.ParameterName(5) != "refParam2")
                {
                    throw new InvalidOperationException("Parameter ordetring missmatch in arguments collection)");
                }

                if ((string)input.Arguments[0] != "inputParam1" || (string)input.Arguments[1] != "inputParam2" || (string)input.Arguments[2] != String.Empty || (string)input.Arguments[3] != String.Empty || (string)input.Arguments[4] != "refParam1" || (string)input.Arguments[5] != "refParam2")
                {
                    throw new InvalidOperationException("Parameter values not reflected correctly in arguments collection)");
                }
                
                if (!input.Arguments.GetParameterInfo(0).IsIn || !input.Arguments.GetParameterInfo(1).IsIn)
                {
                    throw new InvalidOperationException("Parameter values direction for in params not reflected correctly in arguments collection)");
                }
                
                if (!input.Arguments.GetParameterInfo(2).IsOut || !input.Arguments.GetParameterInfo(3).IsOut)
                {
                    throw new InvalidOperationException("Parameter values direction for out params not reflected correctly in arguments collection)");
                }
                
                if ((!input.Arguments.GetParameterInfo(4).IsIn && !input.Arguments.GetParameterInfo(4).IsOut) || !input.Arguments.GetParameterInfo(5).IsOut || !input.Arguments.GetParameterInfo(5).IsIn)
                {
                    throw new InvalidOperationException("Parameter values direction for ref params not reflected correctly in arguments collection)");
                }
            }
            else 
            {
                throw new InvalidOperationException("arguments collection parameter count mismatch");
            }

            IMethodReturn returnValue = getNext().Invoke(input, getNext);

            if (input.Arguments.Count == 6)
            {
                if (input.Arguments.ParameterName(0) != "inputParam1" || input.Arguments.ParameterName(1) != "inputParam2" || input.Arguments.ParameterName(2) != "outParam1" || input.Arguments.ParameterName(3) != "outParam2" || input.Arguments.ParameterName(4) != "refParam1" || input.Arguments.ParameterName(5) != "refParam2")
                {
                    throw new InvalidOperationException("Parameter ordetring missmatch in arguments collection)");
                }
            
                if ((string)input.Arguments[2] != "inside target method outparam1" || (string)input.Arguments[3] != "inside target method outparam2")
                {
                    throw new InvalidOperationException("Out Parameter values not reflected correctly in arguments collection)");
                }

                if ((string)input.Arguments[4] != "refParam1inside target method refParam1" || (string)input.Arguments[5] != "refParam2inside target method refParam2")
                {
                    throw new InvalidOperationException("ref Parameter values not reflected correctly in arguments collection)");
                }
            }

            return returnValue;
        }

        public int Order { get; set; }
    }

    public class MyCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            //never gets here
            return getNext()(input, getNext);
        }

        public int Order { get; set; }
    }
}