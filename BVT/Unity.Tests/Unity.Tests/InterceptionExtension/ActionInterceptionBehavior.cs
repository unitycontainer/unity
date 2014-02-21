// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Tests.InterceptionExtension
{
    public class ActionInterceptionBehavior : IInterceptionBehavior
    {
        private Action action;

        public ActionInterceptionBehavior(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;
        }

        #region IInterceptionBehavior Members

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            action.Invoke();

            return getNext()(input, getNext);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        #endregion

        #region IInterceptionBehavior Members

        public bool WillExecute
        {
            get { return true; }
        }

        #endregion
    }
}
