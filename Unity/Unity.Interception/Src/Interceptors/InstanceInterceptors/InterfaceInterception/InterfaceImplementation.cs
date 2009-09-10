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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    class InterfaceImplementation
    {
        private readonly TypeBuilder typeBuilder;
        private readonly Type @interface;
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
        {
            this.typeBuilder = typeBuilder;
            this.@interface = @interface;
            this.proxyInterceptionPipelineField = proxyInterceptionPipelineField;
            this.explicitImplementation = explicitImplementation;
            this.targetField = targetField;
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
                    this.explicitImplementation,
                    count)
                    .AddMethod();
        }
    }
}
