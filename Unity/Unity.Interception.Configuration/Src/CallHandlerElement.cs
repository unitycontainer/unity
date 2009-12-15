using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element representing a call handler.
    /// </summary>
    public class CallHandlerElement : PolicyChildElement
    {
        internal void Configure(IUnityContainer container, PolicyDefinition policyDefinition)
        {
            if(string.IsNullOrEmpty(TypeName))
            {
                policyDefinition.AddCallHandler(Name);
            }
            else
            {
                Type handlerType = TypeResolver.ResolveType(TypeName);
                var injectionMembers =
                    Injection.SelectMany(
                        element => element.GetInjectionMembers(container, typeof (ICallHandler), handlerType, Name));
                policyDefinition.AddCallHandler(handlerType, Name, Lifetime.CreateLifetimeManager(),
                    injectionMembers.ToArray());
            }

        }
    }
}
