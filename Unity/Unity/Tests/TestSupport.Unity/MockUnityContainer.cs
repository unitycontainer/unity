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
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.TestSupport
{
    public interface IConfigOne : IUnityContainerExtensionConfigurator
    {
        IConfigOne SetText(string text);
    }

    public interface IConfigTwo : IUnityContainerExtensionConfigurator
    {
        IConfigTwo SetMessage(string text);
    }

    public class MockUnityContainer : UnityContainerBase, IConfigOne, IConfigTwo
    {
        public List<ConfigurationActionRecord> ConfigActions =
            new List<ConfigurationActionRecord>();

        private Dictionary<Pair<Type, string>, object> objectsToResolve = new Dictionary<Pair<Type, string>, object>();

        public MockUnityContainer AddObjectToResolve(Type t, string name, object instance)
        {
            objectsToResolve[new Pair<Type, string>(t, name)] = instance;
            return this;
        }

        public override IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            ConfigActions.Add(
                ConfigurationActionRecord.RegisterAction(from, to, name, lifetimeManager));
            return this;
        }

        public override IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
        {
            ConfigActions.Add(ConfigurationActionRecord.RegisterInstanceAction(
                                  t,
                                  name,
                                  instance,
                                  lifetime));
            return this;
        }

        public override object Resolve(Type t, string name)
        {
            return objectsToResolve[new Pair<Type, string>(t, name)];
        }

        public override IEnumerable<object> ResolveAll(Type t)
        {
            throw new NotImplementedException();
        }

        public override object BuildUp(Type t, object existing, string name)
        {
            throw new NotImplementedException();
        }

        public override void Teardown(object o)
        {
            throw new NotImplementedException();
        }

        public override IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            ConfigActions.Add(ConfigurationActionRecord.AddExtensionAction(extension.GetType()));
            return this;
        }

        public override object Configure(Type configurationInterface)
        {
            if(configurationInterface.IsAssignableFrom(GetType()))
            {
                return this;
            }
            return null;
        }

        public override IUnityContainer RemoveAllExtensions()
        {
            return this;
        }


        public override IUnityContainer CreateChildContainer()
        {
            throw new NotImplementedException();
        }


        public override IUnityContainer Parent
        {
            get { throw new NotImplementedException(); }
        }

        public override void Dispose()
        {
        }


        public IConfigOne SetText(string text)
        {
            ConfigActions.Add(ConfigurationActionRecord.ConfigureExtensionOneAction(text));
            return this;
        }

        /// <summary>
        /// Retrieve the container instance that we are currently configuring.
        /// </summary>
        public IUnityContainer Container
        {
            get { return this; }
        }

        public IConfigTwo SetMessage(string text)
        {
            ConfigActions.Add(ConfigurationActionRecord.ConfigureExtensionTwoAction(text));
            return this;
        }
    }

    public enum ConfigurationMethod
    {
        Register,
        RegisterInstance,
        AddExtension,
        ConfigExtension1,
        ConfigExtension2
    }

    public class ConfigurationActionRecord
    {
        private ConfigurationMethod configurationMethod;
        private Type typeFrom;
        private Type typeTo;
        private string name;
        private object instance;
        private LifetimeManager lifetime;


        public ConfigurationActionRecord(ConfigurationMethod configurationMethod, Type typeFrom, Type typeTo, string name, object instance, LifetimeManager lifetime)
        {
            this.configurationMethod = configurationMethod;
            this.typeFrom = typeFrom;
            this.typeTo = typeTo;
            this.name = string.IsNullOrEmpty(name) ? null : name;
            this.instance = instance;
            this.lifetime = lifetime;
        }

        public static ConfigurationActionRecord RegisterAction(Type typeFrom, Type typeTo, string name, LifetimeManager lifetime)
        {
            return
                new ConfigurationActionRecord(ConfigurationMethod.Register,
                                              typeFrom,
                                              typeTo,
                                              name,
                                              null,
                                              lifetime);
        }

        public static ConfigurationActionRecord RegisterInstanceAction(Type t, string name, object instance, LifetimeManager lifetimeManager)
        {
            return new ConfigurationActionRecord(ConfigurationMethod.RegisterInstance,
                                                 t,
                                                 null,
                                                 name,
                                                 instance,
                                                 lifetimeManager);
        }

        public static ConfigurationActionRecord AddExtensionAction(Type t)
        {
            return new ConfigurationActionRecord(ConfigurationMethod.AddExtension,
                                                 t,
                                                 null,
                                                 null,
                                                 null,
                                                 null);
        }

        public static ConfigurationActionRecord ConfigureExtensionOneAction(string text)
        {
            return new ConfigurationActionRecord(ConfigurationMethod.ConfigExtension1,
                                                 null,
                                                 null,
                                                 null,
                                                 text,
                                                 null);
        }

        public static ConfigurationActionRecord ConfigureExtensionTwoAction(string text)
        {
            return new ConfigurationActionRecord(ConfigurationMethod.ConfigExtension2,
                                                 null,
                                                 null,
                                                 null,
                                                 text,
                                                 null);
        }

        public ConfigurationMethod ConfigurationMethod
        {
            get { return configurationMethod; }
        }

        public Type TypeFrom
        {
            get { return typeFrom; }
        }

        public Type TypeTo
        {
            get { return typeTo; }
        }

        public string Name
        {
            get { return name; }
        }

        public object Instance
        {
            get { return instance; }
        }

        public LifetimeManager Lifetime
        {
            get { return lifetime; }
        }

        public override bool Equals(object obj)
        {
            if(!(obj is ConfigurationActionRecord))
            {
                return false;
            }
            return this == (ConfigurationActionRecord)obj;
        }


        public override int GetHashCode()
        {
            return configurationMethod.GetHashCode() ^
                   typeFrom.GetHashCode() ^
                   typeTo.GetHashCode() ^
                   name.GetHashCode() ^
                   (instance != null ? instance.GetHashCode() : 0) ^
                   (lifetime != null ? lifetime.GetHashCode() : 0);
        }

        public static bool operator ==(ConfigurationActionRecord left, ConfigurationActionRecord right)
        {
            return left.configurationMethod == right.configurationMethod &&
                   left.TypeFrom == right.typeFrom &&
                   left.typeTo == right.typeTo &&
                   left.name == right.name &&
                   Equals(left.instance, right.instance) &&
                   ((left.lifetime == null && right.lifetime == null) ||
                    Equals(left.lifetime.GetType(), right.lifetime.GetType()));
        }

        public static bool operator !=(ConfigurationActionRecord left, ConfigurationActionRecord right)
        {
            return !( left == right );
        }
    }
}
