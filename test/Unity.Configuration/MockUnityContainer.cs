// TODO: Verify
//// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//using System;
//using System.Collections.Generic;
//using System.Reflection;


//namespace Unity.Tests.TestDoubles
//{
//    internal interface IConfigOne : IUnityContainerExtensionConfigurator
//    {
//        IConfigOne SetText(string text);
//    }

//    internal interface IConfigTwo : IUnityContainerExtensionConfigurator
//    {
//        IConfigTwo SetMessage(string text);
//    }

//    // A faked up unity container that records which methods were
//    // called on it, and nothing else.
//    internal class MockUnityContainer : IUnityContainer, IConfigOne, IConfigTwo
//    {
//        public List<ConfigurationActionRecord> ConfigActions =
//            new List<ConfigurationActionRecord>();

//        public IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
//        {
//            ConfigActions.Add(
//                ConfigurationActionRecord.RegisterAction(from, to, name, lifetimeManager));
//            return this;
//        }

//        public IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
//        {
//            ConfigActions.Add(ConfigurationActionRecord.RegisterInstanceAction(
//                                  t,
//                                  name,
//                                  instance,
//                                  lifetime));
//            return this;
//        }

//        public object Resolve(Type t, string name)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<object> ResolveAll(Type t)
//        {
//            throw new NotImplementedException();
//        }

//        public object BuildUp(Type t, object existing, string name)
//        {
//            throw new NotImplementedException();
//        }

//        public void Teardown(object o)
//        {
//            throw new NotImplementedException();
//        }

//        public IUnityContainer AddExtension(UnityContainerExtension extension)
//        {
//            ConfigActions.Add(ConfigurationActionRecord.AddExtensionAction(extension.GetType()));
//            return this;
//        }

//        public object Configure(Type configurationInterface)
//        {
//            if (configurationInterface.GetTypeInfo().IsAssignableFrom(this.GetType().GetTypeInfo()))
//            {
//                return this;
//            }
//            return null;
//        }

//        public IUnityContainer RemoveAllExtensions()
//        {
//            return this;
//        }

//        public void Dispose()
//        {
//        }

//        public IConfigOne SetText(string text)
//        {
//            ConfigActions.Add(ConfigurationActionRecord.ConfigureExtensionOneAction(text));
//            return this;
//        }

//        /// <summary>
//        /// Retrieve the container instance that we are currently configuring.
//        /// </summary>
//        public IUnityContainer Container
//        {
//            get { return this; }
//        }

//        public IConfigTwo SetMessage(string text)
//        {
//            ConfigActions.Add(ConfigurationActionRecord.ConfigureExtensionTwoAction(text));
//            return this;
//        }

//        public IUnityContainer CreateChildContainer()
//        {
//            throw new Exception("The method or operation is not implemented.");
//        }

//        public IUnityContainer Parent
//        {
//            get { throw new Exception("The method or operation is not implemented."); }
//        }

//        #region IUnityContainer Members

//        public object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides)
//        {
//            throw new NotImplementedException();
//        }

//        public object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<Microsoft.Practices.Unity.ContainerRegistration> Registrations
//        {
//            get { throw new NotImplementedException(); }
//        }

//        #endregion
//    }

//    public enum ConfigurationMethod
//    {
//        Register,
//        RegisterInstance,
//        AddExtension,
//        ConfigExtension1,
//        ConfigExtension2
//    }

//    internal class ConfigurationActionRecord
//    {
//        private ConfigurationMethod configurationMethod;
//        private Type typeFrom;
//        private Type typeTo;
//        private string name;
//        private object instance;
//        private LifetimeManager lifetime;

//        public ConfigurationActionRecord(ConfigurationMethod configurationMethod, Type typeFrom, Type typeTo, string name, object instance, LifetimeManager lifetime)
//        {
//            this.configurationMethod = configurationMethod;
//            this.typeFrom = typeFrom;
//            this.typeTo = typeTo;
//            this.name = string.IsNullOrEmpty(name) ? null : name;
//            this.instance = instance;
//            this.lifetime = lifetime;
//        }

//        public static ConfigurationActionRecord RegisterAction(Type typeFrom, Type typeTo, string name, LifetimeManager lifetime)
//        {
//            return
//                new ConfigurationActionRecord(TestDoubles.ConfigurationMethod.Register,
//                                              typeFrom,
//                                              typeTo,
//                                              name,
//                                              null,
//                                              lifetime);
//        }

//        public static ConfigurationActionRecord RegisterInstanceAction(Type t, string name, object instance, LifetimeManager lifetimeManager)
//        {
//            return new ConfigurationActionRecord(TestDoubles.ConfigurationMethod.RegisterInstance,
//                                                 t,
//                                                 null,
//                                                 name,
//                                                 instance,
//                                                 lifetimeManager);
//        }

//        public static ConfigurationActionRecord AddExtensionAction(Type t)
//        {
//            return new ConfigurationActionRecord(TestDoubles.ConfigurationMethod.AddExtension,
//                                                 t,
//                                                 null,
//                                                 null,
//                                                 null,
//                                                 null);
//        }

//        public static ConfigurationActionRecord ConfigureExtensionOneAction(string text)
//        {
//            return new ConfigurationActionRecord(TestDoubles.ConfigurationMethod.ConfigExtension1,
//                null,
//                null,
//                null,
//                text,
//                null);
//        }

//        public static ConfigurationActionRecord ConfigureExtensionTwoAction(string text)
//        {
//            return new ConfigurationActionRecord(TestDoubles.ConfigurationMethod.ConfigExtension2,
//                null,
//                null,
//                null,
//                text,
//                null);
//        }

//        public ConfigurationMethod ConfigurationMethod
//        {
//            get { return configurationMethod; }
//        }

//        public Type TypeFrom
//        {
//            get { return typeFrom; }
//        }

//        public Type TypeTo
//        {
//            get { return typeTo; }
//        }

//        public string Name
//        {
//            get { return name; }
//        }

//        public object Instance
//        {
//            get { return instance; }
//        }

//        public LifetimeManager Lifetime
//        {
//            get { return lifetime; }
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is ConfigurationActionRecord))
//            {
//                return false;
//            }
//            return this == (ConfigurationActionRecord)obj;
//        }

//        public override int GetHashCode()
//        {
//            return configurationMethod.GetHashCode() ^
//                   typeFrom.GetHashCode() ^
//                   typeTo.GetHashCode() ^
//                   name.GetHashCode() ^
//                   (instance != null ? instance.GetHashCode() : 0) ^
//                   (lifetime != null ? lifetime.GetHashCode() : 0);
//        }

//        public static bool operator ==(ConfigurationActionRecord left, ConfigurationActionRecord right)
//        {
//            return left.configurationMethod == right.configurationMethod &&
//                   left.TypeFrom == right.typeFrom &&
//                   left.typeTo == right.typeTo &&
//                   left.name == right.name &&
//                   Object.Equals(left.instance, right.instance) &&
//                   ((left.lifetime == null && right.lifetime == null) ||
//                   Object.Equals(left.lifetime.GetType(), right.lifetime.GetType()));
//        }

//        public static bool operator !=(ConfigurationActionRecord left, ConfigurationActionRecord right)
//        {
//            return !(left == right);
//        }
//    }
//}
