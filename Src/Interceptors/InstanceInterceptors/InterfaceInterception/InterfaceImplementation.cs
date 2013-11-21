// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    class InterfaceImplementation
    {
        private readonly TypeBuilder typeBuilder;
        private readonly Type @interface;
        private readonly Type targetInterface;
        private readonly GenericParameterMapper genericParameterMapper;
        private readonly FieldBuilder proxyInterceptionPipelineField;
        private readonly bool explicitImplementation;
        private readonly FieldBuilder targetField;

        public InterfaceImplementation(
            TypeBuilder typeBuilder,
            Type @interface,
            FieldBuilder proxyInterceptionPipelineField,
            bool explicitImplementation)
            : this(typeBuilder, @interface, proxyInterceptionPipelineField, explicitImplementation, null)
        { }

        public InterfaceImplementation(
            TypeBuilder typeBuilder,
            Type @interface,
            FieldBuilder proxyInterceptionPipelineField,
            bool explicitImplementation,
            FieldBuilder targetField)
            : this(typeBuilder, @interface, GenericParameterMapper.DefaultMapper, proxyInterceptionPipelineField, explicitImplementation, targetField)
        { }

        public InterfaceImplementation(
            TypeBuilder typeBuilder,
            Type @interface,
            GenericParameterMapper genericParameterMapper,
            FieldBuilder proxyInterceptionPipelineField,
            bool explicitImplementation,
            FieldBuilder targetField)
        {
            this.typeBuilder = typeBuilder;
            this.@interface = @interface;
            this.genericParameterMapper = genericParameterMapper;
            this.proxyInterceptionPipelineField = proxyInterceptionPipelineField;
            this.explicitImplementation = explicitImplementation;
            this.targetField = targetField;

            if (@interface.IsGenericType)
            {
                // when the @interface is generic we need to get references to its methods though it
                // in this case, the targetInterface is a constructed version using the generic type parameters
                // from the generated type generate type
                var definition = @interface.GetGenericTypeDefinition();
                var mappedParameters = definition.GetGenericArguments().Select(t => genericParameterMapper.Map(t)).ToArray();
                this.targetInterface = definition.MakeGenericType(mappedParameters);
            }
            else
            {
                this.targetInterface = @interface;
            }
        }

        public int Implement(HashSet<Type> implementedInterfaces, int memberCount)
        {
            if (implementedInterfaces.Contains(this.@interface))
            {
                return memberCount;
            }

            implementedInterfaces.Add(this.@interface);

            typeBuilder.AddInterfaceImplementation(this.@interface);

            foreach (MethodInfo method in MethodsToIntercept())
            {
                OverrideMethod(method, memberCount++);
            }

            foreach (PropertyInfo property in PropertiesToIntercept())
            {
                OverrideProperty(property, memberCount++);
            }

            foreach (EventInfo @event in EventsToIntercept())
            {
                OverrideEvent(@event, memberCount++);
            }

            foreach (var @extendedInterface in this.@interface.GetInterfaces())
            {
                memberCount =
                    new InterfaceImplementation(
                        this.typeBuilder,
                        @extendedInterface,
                        new GenericParameterMapper(@extendedInterface, this.genericParameterMapper),
                        this.proxyInterceptionPipelineField,
                        this.explicitImplementation,
                        this.targetField)
                        .Implement(implementedInterfaces, memberCount);
            }

            return memberCount;
        }

        private IEnumerable<MethodInfo> MethodsToIntercept()
        {
            foreach (MethodInfo method in
                this.@interface.GetMethods(
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!method.IsSpecialName)
                {
                    yield return method;
                }
            }
        }

        private void OverrideMethod(MethodInfo method, int methodNum)
        {
            new InterfaceMethodOverride(
                this.typeBuilder,
                this.proxyInterceptionPipelineField,
                this.targetField,
                method,
                this.targetInterface,
                this.genericParameterMapper,
                this.explicitImplementation,
                methodNum)
                .AddMethod();
        }

        private IEnumerable<PropertyInfo> PropertiesToIntercept()
        {
            return this.@interface.GetProperties(
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private void OverrideProperty(PropertyInfo property, int count)
        {
            MethodBuilder getMethod = OverridePropertyMethod(property.GetGetMethod(), count);
            MethodBuilder setMethod = OverridePropertyMethod(property.GetSetMethod(), count);
            AddPropertyDefinition(property, getMethod, setMethod);
        }

        private void AddPropertyDefinition(PropertyInfo property, MethodBuilder getMethod, MethodBuilder setMethod)
        {
            PropertyBuilder newProperty =
                this.typeBuilder.DefineProperty(
                    property.Name,
                    property.Attributes,
                    property.PropertyType,
                    property.GetIndexParameters().Select(param => param.ParameterType).ToArray());

            if (getMethod != null)
            {
                newProperty.SetGetMethod(getMethod);
            }

            if (setMethod != null)
            {
                newProperty.SetSetMethod(setMethod);
            }
        }

        private MethodBuilder OverridePropertyMethod(MethodInfo method, int count)
        {
            return method == null
                ? null
                : new InterfaceMethodOverride(
                    this.typeBuilder,
                    this.proxyInterceptionPipelineField,
                    this.targetField,
                    method,
                    this.targetInterface,
                    this.genericParameterMapper,
                    this.explicitImplementation,
                    count)
                    .AddMethod();
        }

        private IEnumerable<EventInfo> EventsToIntercept()
        {
            return this.@interface.GetEvents(
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private void OverrideEvent(EventInfo @event, int count)
        {
            MethodBuilder addMethod = OverrideEventMethod(@event.GetAddMethod(), count);
            MethodBuilder removeMethod = OverrideEventMethod(@event.GetRemoveMethod(), count);
            AddEventDefinition(@event, addMethod, removeMethod);
        }

        private void AddEventDefinition(EventInfo @event, MethodBuilder addMethod, MethodBuilder removeMethod)
        {
            EventBuilder newEvent = this.typeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);

            if (addMethod != null)
            {
                newEvent.SetAddOnMethod(addMethod);
            }

            if (removeMethod != null)
            {
                newEvent.SetRemoveOnMethod(removeMethod);
            }
        }

        private MethodBuilder OverrideEventMethod(MethodInfo method, int count)
        {
            return method == null
                ? null
                : new InterfaceMethodOverride(
                    this.typeBuilder,
                    this.proxyInterceptionPipelineField,
                    this.targetField,
                    method,
                    this.targetInterface,
                    this.genericParameterMapper,
                    this.explicitImplementation,
                    count)
                    .AddMethod();
        }
    }
}
