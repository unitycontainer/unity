using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Practices.Unity;

namespace UnityDefaultValueExtension
{
    /// <summary>
    /// Extension method based implementation
    /// </summary>
    public static class AOPUnityDefaultValueExtender
    {
        /// <summary>
        /// Adds required dependency registrations required to intialize properties
        /// <param name="_this">The type.</param>
        /// <param name="injectionMembers">Other dependencies.</param>
        /// </summary>
        public static InjectionMember[] DefaultValueDependencies(this Type _this)
        {
            ArrayList defValDepends = new ArrayList();

            foreach (PropertyInfo prop in _this.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // Skip Read Only properties
                if (!prop.CanWrite)
                    continue;

                // If DependencyAttribute is present, no need to do anything else
                if (0 < prop.GetCustomAttributes(typeof(DependencyAttribute), false).Length)
                    continue;

                // Get default attribute and register dependency
                DefaultValueAttribute[] defValue = prop.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                                                                            as DefaultValueAttribute[];
                if (0 == defValue.Length)
                    continue;

                defValDepends.Add(new InjectionProperty(prop.Name,
                    (object)Convert.ChangeType(defValue[0].Value, prop.PropertyType)));
            }

            // If added any properties, merge and return
            if (0 < defValDepends.Count)
            {
                //defValDepends.InsertRange(0, );
                return (InjectionMember[])defValDepends.ToArray(typeof(InjectionMember));
            }

            return null;
        }

        /// <summary>
        /// Register Type and Interface with Unity containainer and sets up properties with
        /// DefaultValueAttribute as dependency property. Unity framework initializes these
        /// with constant value specified in the attribute.
        /// <param name="TFrom">The type.</param>
        /// <param name="TTo">A concrete mplementation of the type.</param>
        /// <param name="_this">Unity container.</param>
        /// </summary>
        public static IUnityContainer RegisterTypeWithDefaultValues<TFrom, TTo>(this IUnityContainer _this)
            where TTo : TFrom
        {
            // Register the type with the container
            return _this.RegisterType<TFrom, TTo>(typeof(TTo).DefaultValueDependencies());
        }
    }

    
    
    /// <summary>
    /// Extension initializing resolved objects with values from DefaultValueAttributes
    /// </summary>
    public class UnityDefaultValueExtender : UnityContainerExtension, IUnityDefaultValueExtender
    {
        /// <summary>
        /// Initializes this extension by attaching to registration event
        /// </summary>
        protected override void Initialize()
        {
            base.Context.Registering += new EventHandler<RegisterEventArgs>(this.OnRegister);
        }

        /// <summary>
        /// Processes registration event. Adds policies to initialize properties with constant 
        /// values retrieved from DefaultValueAttributes
        /// </summary>
        private void OnRegister(object sender, RegisterEventArgs e)
        {
            ArrayList defValDepends = new ArrayList();
            IUnityContainer container = sender as IUnityContainer;

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(e.TypeTo))
            {
                // Skip Read Only properties
                if (prop.IsReadOnly)
                    continue;

                // If DependencyAttribute is present, no need to do anything else
                if (null != prop.Attributes[typeof(DependencyAttribute)])
                    continue;

                // Get default attribute and register dependency
                DefaultValueAttribute defValue = prop.Attributes[typeof(DefaultValueAttribute)]
                                                                as DefaultValueAttribute;
                if (null == defValue)
                    continue;

#if LEGACY_CODE // Code for Microsoft Enterprise Libraries prior to 5 (1.2 ... 4.1)
                (new InjectionProperty(prop.DisplayName, (object)Convert.ChangeType(defValue.Value, prop.PropertyType)))
                                        .AddPolicies(e.TypeTo, e.Name, base.Context.Policies);

#else           // Code for Microsoft Enterprise Library 5.xx and up
                (new InjectionProperty(prop.DisplayName, (object)Convert.ChangeType(defValue.Value, prop.PropertyType)))
                                        .AddPolicies(e.TypeFrom, e.TypeTo, e.Name, base.Context.Policies);
#endif
            }
        }
    }



    /// <summary>
    /// Configuration Interface used to configure this extension
    /// </summary>
    public interface IUnityDefaultValueExtender : IUnityContainerExtensionConfigurator { }
}
