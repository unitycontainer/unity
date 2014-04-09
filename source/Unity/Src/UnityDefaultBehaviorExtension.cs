// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    public class UnityDefaultBehaviorExtension : UnityContainerExtension
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        protected override void Initialize()
        {
            Context.Registering += this.OnRegister;
            Context.RegisteringInstance += this.OnRegisterInstance;

            Container.RegisterInstance(Container, new ContainerLifetimeManager());
        }

        /// <summary>
        /// Remove the default behavior from the container.
        /// </summary>
        public override void Remove()
        {
            Context.Registering -= this.OnRegister;
            Context.RegisteringInstance -= this.OnRegisterInstance;
        }

        private void OnRegister(object sender, RegisterEventArgs e)
        {
            Context.RegisterNamedType(e.TypeFrom ?? e.TypeTo, e.Name);
            if (e.TypeFrom != null)
            {
                if (e.TypeFrom.GetTypeInfo().IsGenericTypeDefinition && e.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
                {
                    Context.Policies.Set<IBuildKeyMappingPolicy>(
                        new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(e.TypeTo, e.Name)),
                        new NamedTypeBuildKey(e.TypeFrom, e.Name));
                }
                else
                {
                    Context.Policies.Set<IBuildKeyMappingPolicy>(
                        new BuildKeyMappingPolicy(new NamedTypeBuildKey(e.TypeTo, e.Name)),
                        new NamedTypeBuildKey(e.TypeFrom, e.Name));
                }
            }
            if (e.LifetimeManager != null)
            {
                this.SetLifetimeManager(e.TypeTo, e.Name, e.LifetimeManager);
            }
        }

        private void OnRegisterInstance(object sender, RegisterInstanceEventArgs e)
        {
            Context.RegisterNamedType(e.RegisteredType, e.Name);
            this.SetLifetimeManager(e.RegisteredType, e.Name, e.LifetimeManager);
            NamedTypeBuildKey identityKey = new NamedTypeBuildKey(e.RegisteredType, e.Name);
            Context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(identityKey), identityKey);
            e.LifetimeManager.SetValue(e.Instance);
        }

        private void SetLifetimeManager(Type lifetimeType, string name, LifetimeManager lifetimeManager)
        {
            if (lifetimeManager.InUse)
            {
                throw new InvalidOperationException(Resources.LifetimeManagerInUse);
            }
            if (lifetimeType.GetTypeInfo().IsGenericTypeDefinition)
            {
                LifetimeManagerFactory factory =
                    new LifetimeManagerFactory(Context, lifetimeManager.GetType());
                Context.Policies.Set<ILifetimeFactoryPolicy>(factory,
                    new NamedTypeBuildKey(lifetimeType, name));
            }
            else
            {
                lifetimeManager.InUse = true;
                Context.Policies.Set<ILifetimePolicy>(lifetimeManager,
                    new NamedTypeBuildKey(lifetimeType, name));
                if (lifetimeManager is IDisposable)
                {
                    Context.Lifetime.Add(lifetimeManager);
                }
            }
        }

        // Works like the ExternallyControlledLifetimeManager, but uses regular instead of weak references
        private class ContainerLifetimeManager : LifetimeManager
        {
            private object value;

            public override object GetValue()
            {
                return this.value;
            }

            public override void SetValue(object newValue)
            {
                this.value = newValue;
            }

            public override void RemoveValue()
            {
            }
        }
    }
}
