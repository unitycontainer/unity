// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Xml;
using Microsoft.Practices.Unity.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    [TestClass]
    public class When_SavingModifiedConfigurationSection
    {
        private static UnityConfigurationSection SerializeAndLoadSection(string filename, Action<UnityConfigurationSection> sectionInitializer)
        {
            var serializer = new ConfigSerializer(filename);
            var section = new UnityConfigurationSection();
            sectionInitializer(section);
            serializer.Save("unity", section);

            var loadedConfiguration = serializer.Load();
            return (UnityConfigurationSection)loadedConfiguration.GetSection("unity");
        }

        [TestMethod]
        public void Then_EmptySectionCanBeSavedAndReloaded()
        {
            var section = SerializeAndLoadSection("EmptySection.config",
                s => { });
            Assert.IsNotNull(section);
        }

        [TestMethod]
        public void Then_OneAddedAliasIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeAliases.config",
                s => s.TypeAliases.Add(new AliasElement("mockdb", typeof(MockDatabase))));

            Assert.AreEqual(1, loadedSection.TypeAliases.Count);
            Assert.AreEqual(typeof(MockDatabase).AssemblyQualifiedName, loadedSection.TypeAliases["mockdb"]);
        }

        [TestMethod]
        public void Then_TwoAddedAliasesAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeAliases.config", s =>
            {
                s.TypeAliases.Add(new AliasElement("mockdb", typeof(MockDatabase)));
                s.TypeAliases.Add(new AliasElement("ilog", typeof(ILogger)));
            });

            Assert.AreEqual(2, loadedSection.TypeAliases.Count);
            Assert.AreEqual(typeof(MockDatabase).AssemblyQualifiedName, loadedSection.TypeAliases["mockdb"]);
            Assert.AreEqual(typeof(ILogger).AssemblyQualifiedName, loadedSection.TypeAliases["ilog"]);
        }

        [TestMethod]
        public void Then_TwoNamespacesAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeNamespaces.config", s =>
            {
                s.TypeAliases.Add(new AliasElement("mockdb", typeof(MockDatabase)));

                s.Namespaces.Add(new NamespaceElement { Name = "System" });
                s.Namespaces.Add(new NamespaceElement { Name = "System.Collections" });
            });

            loadedSection.Namespaces.Select(ns => ns.Name)
                .AssertContainsExactly("System", "System.Collections");
        }

        [TestMethod]
        public void Then_AssembliesAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeAssemblies.config", s =>
            {
                s.TypeAliases.Add(new AliasElement("mockdb", typeof(MockDatabase)));
                s.Namespaces.Add(new NamespaceElement { Name = "System" });

                s.Assemblies.Add(new AssemblyElement { Name = typeof(int).Assembly.FullName });
                s.Assemblies.Add(new AssemblyElement { Name = typeof(XmlWriter).Assembly.FullName });
            });

            loadedSection.Assemblies.Select(asm => asm.Name)
                .AssertContainsExactly(
                    typeof(int).Assembly.FullName,
                    typeof(XmlWriter).Assembly.FullName);
        }

        [TestMethod]
        public void Then_SectionExtensionsAreSerialized()
        {
            string extensionTypeName = typeof(TestSectionExtension).AssemblyQualifiedName;

            var loadedSection = SerializeAndLoadSection("SerializeSectionExtensions.config", s =>
            {
                s.SectionExtensions.Add(new SectionExtensionElement { TypeName = extensionTypeName });
                s.SectionExtensions.Add(new SectionExtensionElement { TypeName = extensionTypeName, Prefix = "p1" });
            });

            loadedSection.SectionExtensions.Select(se => se.TypeName)
                .AssertContainsExactly(extensionTypeName, extensionTypeName);

            loadedSection.SectionExtensions.Select(se => se.Prefix)
                .AssertContainsExactly(String.Empty, "p1");
        }

        [TestMethod]
        public void Then_OneContainerIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeContainers.config", s =>
            {
                var container = new ContainerElement();
                s.Containers.Add(container);
            });

            Assert.AreEqual(1, loadedSection.Containers.Count);
        }

        [TestMethod]
        public void Then_TwoNamedContainersAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeContainers.config", s =>
            {
                s.Containers.Add(new ContainerElement { Name = "containerOne" });
                s.Containers.Add(new ContainerElement { Name = "containerTwo" });
            });

            loadedSection.Containers.Select(c => c.Name)
                .AssertContainsExactly("containerOne", "containerTwo");
        }

        [TestMethod]
        public void Then_ContainerWithExtensionsIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeContainers.config", s =>
            {
                var containerElement = new ContainerElement();
                containerElement.Extensions.Add(new ContainerExtensionElement { TypeName = "extensionOne" });
                containerElement.Extensions.Add(new ContainerExtensionElement { TypeName = "extensionTwo" });
                s.Containers.Add(containerElement);
            });

            loadedSection.Containers.Default.Extensions.Select(ce => ce.TypeName)
                .AssertContainsExactly("extensionOne", "extensionTwo");
        }

        [TestMethod]
        public void Then_ContainerWithTypeMappingsIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeContainers.config", s =>
            {
                var containerElement = new ContainerElement();
                containerElement.Registrations.Add(new RegisterElement { TypeName = "SourceOne", MapToName = "DestOne", Name = "NameOne" });
                containerElement.Registrations.Add(new RegisterElement
                {
                    TypeName = "SourceTwo",
                    MapToName = "DestTwo",
                    Name = "NameTwo"
                });
                containerElement.Registrations.Add(new RegisterElement
                {
                    TypeName = "SourceThree",
                    MapToName = "DestThree"
                });
                s.Containers.Add(containerElement);
            });

            loadedSection.Containers.Default.Registrations.Select(r => r.TypeName)
                .AssertContainsExactly("SourceOne", "SourceTwo", "SourceThree");
            loadedSection.Containers.Default.Registrations.Select(r => r.MapToName)
                .AssertContainsExactly("DestOne", "DestTwo", "DestThree");
            loadedSection.Containers.Default.Registrations.Select(r => r.Name)
                .AssertContainsExactly("NameOne", "NameTwo", String.Empty);
        }

        [TestMethod]
        public void Then_RegistrationWithLifetimeIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeContainers.config", s =>
            {
                var containerElement = new ContainerElement();

                var reg1 = new RegisterElement
                {
                    TypeName = "MyType"
                };
                var reg2 = new RegisterElement
                {
                    TypeName = "MyOtherType",
                    Lifetime = new LifetimeElement
                    {
                        TypeName = "SomeCustomLifetime",
                        Value = "input value",
                        TypeConverterTypeName = "CustomLifetimeConverter"
                    }
                };

                var reg3 = new RegisterElement
                {
                    TypeName = "StillAnotherType",
                    Lifetime = new LifetimeElement
                    {
                        TypeName = "singleton"
                    }
                };

                containerElement.Registrations.Add(reg1);
                containerElement.Registrations.Add(reg2);
                containerElement.Registrations.Add(reg3);

                s.Containers.Add(containerElement);
            });

            var loadedReg0 = loadedSection.Containers.Default.Registrations[0];
            var loadedReg1 = loadedSection.Containers.Default.Registrations[1];
            var loadedReg2 = loadedSection.Containers.Default.Registrations[2];

            Assert.IsNotNull(loadedReg0.Lifetime);
            Assert.AreEqual(String.Empty, loadedReg0.Lifetime.TypeName);

            Assert.IsNotNull(loadedReg1.Lifetime);
            Assert.AreEqual("SomeCustomLifetime", loadedReg1.Lifetime.TypeName);
            Assert.AreEqual("input value", loadedReg1.Lifetime.Value);
            Assert.AreEqual("CustomLifetimeConverter", loadedReg1.Lifetime.TypeConverterTypeName);

            Assert.IsNotNull(loadedReg2.Lifetime);
            Assert.AreEqual("singleton", loadedReg2.Lifetime.TypeName);
            Assert.AreEqual(String.Empty, loadedReg2.Lifetime.TypeConverterTypeName);
        }

        [TestMethod]
        public void Then_RegistrationWithInstancesIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeInstances.config", s =>
            {
                var inst0 = new InstanceElement
                {
                    TypeName = "int",
                    Value = "42"
                };

                var inst1 = new InstanceElement
                {
                    TypeName = "DateTime",
                    Value = "today",
                    TypeConverterTypeName = "RelativeDateTextConverter"
                };

                var inst2 = new InstanceElement
                {
                    TypeName = "string",
                    Value = "hello",
                    Name = "AString"
                };

                var containerElement = new ContainerElement();
                containerElement.Instances.Add(inst0);
                containerElement.Instances.Add(inst1);
                containerElement.Instances.Add(inst2);

                s.Containers.Add(containerElement);
            });

            var instances = loadedSection.Containers.Default.Instances;

            instances.Select(i => i.TypeName)
                .AssertContainsExactly("int", "DateTime", "string");
            instances.Select(i => i.Value)
                .AssertContainsExactly("42", "today", "hello");
            instances.Select(i => i.TypeConverterTypeName)
                .AssertContainsExactly(String.Empty, "RelativeDateTextConverter", String.Empty);
            instances.Select(i => i.Name)
                .AssertContainsExactly(String.Empty, String.Empty, "AString");
        }

        [TestMethod]
        public void Then_RegistrationsWithConstructorsIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeCtors.config", FillSectionWithConstructors);

            var zeroArg = loadedSection.Containers.Default.Registrations.Where(r => r.Name == "zeroArg").First();

            Assert.AreEqual(1, zeroArg.InjectionMembers.Count);
            Assert.IsInstanceOfType(zeroArg.InjectionMembers[0], typeof(ConstructorElement));

            var oneArg = loadedSection.Containers.Default.Registrations.Where(r => r.Name == "oneArg").First();
            Assert.AreEqual(1, oneArg.InjectionMembers.Count);
            Assert.IsInstanceOfType(oneArg.InjectionMembers[0], typeof(ConstructorElement));
        }

        [TestMethod]
        public void Then_ConstructorParametersAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeCtors.config", FillSectionWithConstructors);

            var zeroArg = loadedSection.Containers.Default.Registrations.Where(r => r.Name == "zeroArg").First();
            var zeroArgCtor = (ConstructorElement)zeroArg.InjectionMembers[0];

            Assert.AreEqual(0, zeroArgCtor.Parameters.Count);

            var oneArg = loadedSection.Containers.Default.Registrations.Where(r => r.Name == "oneArg").First();
            var oneArgCtor = (ConstructorElement)oneArg.InjectionMembers[0];

            Assert.AreEqual(1, oneArgCtor.Parameters.Count);
            Assert.AreEqual("intParam", oneArgCtor.Parameters[0].Name);
        }

        [TestMethod]
        public void Then_ConstructorParametrValuesAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializeCtors.config", FillSectionWithConstructors);

            var oneArg = loadedSection.Containers.Default.Registrations.Where(r => r.Name == "oneArg").First();
            var oneArgCtor = (ConstructorElement)oneArg.InjectionMembers[0];
            var intParamArg = oneArgCtor.Parameters[0];
            var intParamValue = intParamArg.Value as ValueElement;

            Assert.IsNotNull(intParamValue);
            Assert.AreEqual(intParamValue.Value, "23");
        }

        [TestMethod]
        public void Then_NamedDependencyValueIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<DependencyElement>("namedDependency");

            Assert.AreEqual("someName", parameterValue.Name);
        }

        [TestMethod]
        public void Then_TypedDependencyIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<DependencyElement>("typedDependency");

            Assert.AreEqual("SomeOtherType", parameterValue.TypeName);
        }

        [TestMethod]
        public void Then_ValueIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<ValueElement>("valueDependency");

            Assert.AreEqual("someValue", parameterValue.Value);
        }

        [TestMethod]
        public void Then_ValueWithTypeConverterIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<ValueElement>("valueWithTypeConverter");

            Assert.AreEqual("someValue", parameterValue.Value);
            Assert.AreEqual("MyConverter", parameterValue.TypeConverterTypeName);
        }

        [TestMethod]
        public void Then_OptionalValueIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<OptionalElement>("optionalValue");

            Assert.AreEqual("dependencyKey", parameterValue.Name);
            Assert.AreEqual("DependencyType", parameterValue.TypeName);
        }

        [TestMethod]
        public void Then_EmptyArrayValueIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<ArrayElement>("emptyArrayValue");

            Assert.AreEqual(0, parameterValue.Values.Count);
        }

        [TestMethod]
        public void Then_TypedArrayValueIsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<ArrayElement>("typedEmptyArrayValue");

            Assert.AreEqual(0, parameterValue.Values.Count);
            Assert.AreEqual("MyElementType", parameterValue.TypeName);
        }

        [TestMethod]
        public void Then_ArrayValueWithElementsGetsSerialized()
        {
            var parameterValue = GetRoundTrippedParameter<ArrayElement>("arrayWithValues");

            Assert.AreEqual(3, parameterValue.Values.Count);
            parameterValue.Values.Select(p => p.GetType())
                .AssertContainsExactly(typeof(DependencyElement), typeof(ValueElement), typeof(DependencyElement));

            parameterValue.Values.OfType<DependencyElement>()
                .Select(p => p.Name)
                .AssertContainsExactly(String.Empty, "dependencyName");

            parameterValue.Values.OfType<ValueElement>()
                .Select(p => p.Value)
                .AssertContainsExactly("something");
        }

        [TestMethod]
        public void Then_SimplePropertyWithDependencyIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializingProperties.config", s =>
            {
                var prop = new PropertyElement() { Name = "MyProp" };
                var reg = new RegisterElement()
                {
                    TypeName = "MyType"
                };
                reg.InjectionMembers.Add(prop);

                var container = new ContainerElement();
                container.Registrations.Add(reg);
                s.Containers.Add(container);
            });

            loadedSection.Containers[0].Registrations[0].InjectionMembers
                .Cast<PropertyElement>()
                .Select(p => p.Name)
                .AssertContainsExactly("MyProp");

            loadedSection.Containers[0].Registrations[0].InjectionMembers
                .Cast<PropertyElement>()
                .Select(p => p.Value.GetType())
                .AssertContainsExactly(typeof(DependencyElement));
        }

        [TestMethod]
        public void Then_MultiplePropertiesWithVaryingValuesAreSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializingProperties.config", s =>
            {
                var prop = new PropertyElement() { Name = "SimpleProp" };
                var prop2 = new PropertyElement
                {
                    Name = "NamedDependencyProp",
                    Value = new DependencyElement
                    {
                        Name = "MyDep"
                    }
                };
                var prop3 = new PropertyElement
                {
                    Name = "OptionalProp",
                    Value = new OptionalElement
                    {
                        Name = "OptionalDep",
                        TypeName = "MyType"
                    }
                };

                var reg = new RegisterElement()
                {
                    TypeName = "MyType"
                };
                reg.InjectionMembers.Add(prop);
                reg.InjectionMembers.Add(prop2);
                reg.InjectionMembers.Add(prop3);

                var container = new ContainerElement();
                container.Registrations.Add(reg);
                s.Containers.Add(container);
            });

            var propertyElements = loadedSection.Containers[0].Registrations[0].InjectionMembers.Cast<PropertyElement>();

            propertyElements.Select(p => p.Name)
                .AssertContainsExactly("SimpleProp", "NamedDependencyProp", "OptionalProp");

            propertyElements.Select(p => p.Value.GetType())
                .AssertContainsExactly(typeof(DependencyElement), typeof(DependencyElement), typeof(OptionalElement));

            Assert.AreEqual("MyDep",
                ((DependencyElement)propertyElements.Where(p => p.Name == "NamedDependencyProp").First().Value).Name);
        }

        [TestMethod]
        public void Then_MethodsGetSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializingMethods.config", s =>
            {
                var method0 = new MethodElement
                {
                    Name = "NoArgsMethod"
                };

                var method1 = new MethodElement
                {
                    Name = "OneArgMethod"
                };

                method1.Parameters.Add(new ParameterElement
                {
                    Name = "BasicDependency"
                });
                method1.Parameters.Add(new ParameterElement
                {
                    Name = "ArrayDependency",
                    Value = new ArrayElement
                    {
                        TypeName = "SomeType"
                    }
                });

                var reg = new RegisterElement
                {
                    TypeName = "MyType"
                };
                reg.InjectionMembers.Add(method0);
                reg.InjectionMembers.Add(method1);
                var container = new ContainerElement();
                container.Registrations.Add(reg);
                s.Containers.Add(container);
            });

            var methods = loadedSection.Containers.Default.Registrations[0].InjectionMembers.Cast<MethodElement>();

            methods.Select(m => m.Name)
                .AssertContainsExactly("NoArgsMethod", "OneArgMethod");

            var oneArgMethod = methods.Where(m => m.Name == "OneArgMethod").First();
            Assert.AreEqual(2, oneArgMethod.Parameters.Count);
        }

        [TestMethod]
        public void Then_SectionWithExtendedContainerConfiguringElementsIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializingExtensionElements.config", s =>
            {
                var extensionElement = new ContainerConfigElementTwo();
                var container = new ContainerElement();
                container.ConfiguringElements.Add(extensionElement);
                s.Containers.Add(container);
                s.SectionExtensions.Add(new SectionExtensionElement
                {
                    TypeName = typeof(TestSectionExtension).AssemblyQualifiedName,
                    Prefix = "pre1"
                });
            });

            loadedSection.Containers.Default.ConfiguringElements
                .Select(e => e.GetType())
                .AssertContainsExactly(typeof(ContainerConfigElementTwo));
        }

        [TestMethod]
        public void Then_SectionWithExtendedInjectionmemberElementsIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializingExtensionElements.config", s =>
            {
                var registration = new RegisterElement
                {
                    TypeName = "string"
                };
                registration.InjectionMembers.Add(new TestInjectionMemberElement());

                var container = new ContainerElement();
                container.Registrations.Add(registration);
                s.Containers.Add(container);
                s.SectionExtensions.Add(new SectionExtensionElement
                {
                    TypeName = typeof(TestSectionExtension).AssemblyQualifiedName,
                    Prefix = "pre1"
                });
            });

            loadedSection.Containers.Default.Registrations[0].InjectionMembers
                .Select(i => i.GetType())
                .AssertContainsExactly(typeof(TestInjectionMemberElement));
        }

        [TestMethod]
        public void Then_SectionWithExtendedValueElementsIsSerialized()
        {
            var loadedSection = SerializeAndLoadSection("SerializingExtensionElements.config", s =>
            {
                var registration = new RegisterElement
                {
                    TypeName = "SomeType"
                };

                var prop = new PropertyElement()
                {
                    Name = "MyProp",
                    Value = new SeventeenValueElement()
                };

                registration.InjectionMembers.Add(prop);

                var container = new ContainerElement();
                container.Registrations.Add(registration);
                s.Containers.Add(container);
                s.SectionExtensions.Add(new SectionExtensionElement
                {
                    TypeName = typeof(TestSectionExtension).AssemblyQualifiedName,
                    Prefix = "pre1"
                });
            });

            loadedSection.Containers.Default.Registrations[0].InjectionMembers
                .OfType<PropertyElement>()
                .Select(p => p.Value.GetType())
                .AssertContainsExactly(typeof(SeventeenValueElement));
        }

        private TElement GetRoundTrippedParameter<TElement>(string parameterName)
            where TElement : ParameterValueElement
        {
            var loadedSection = SerializeAndLoadSection("ParameterValues.config", BuildConfigWithValues);
            return FindParameterByName<TElement>(loadedSection, parameterName);
        }

        private TElement FindParameterByName<TElement>(UnityConfigurationSection section, string parameterName)
            where TElement : ParameterValueElement
        {
            var ctor = (ConstructorElement)section.Containers.Default.Registrations[0].InjectionMembers[0];
            return (TElement)ctor.Parameters.Where(p => p.Name == parameterName).First().Value;
        }

        private void BuildConfigWithValues(UnityConfigurationSection s)
        {
            var ctorElement = new ConstructorElement();
            ctorElement.Parameters.Add(new ParameterElement
            {
                Name = "defaultDependency"
            });
            ctorElement.Parameters.Add(new ParameterElement
            {
                Name = "namedDependency",
                Value = new DependencyElement
                {
                    Name = "someName"
                }
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "typedDependency",
                Value = new DependencyElement()
                {
                    TypeName = "SomeOtherType"
                }
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "valueDependency",
                Value = new ValueElement
                {
                    Value = "someValue"
                }
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "valueWithTypeConverter",
                Value = new ValueElement
                {
                    Value = "someValue",
                    TypeConverterTypeName = "MyConverter"
                }
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "optionalValue",
                Value = new OptionalElement()
                {
                    Name = "dependencyKey",
                    TypeName = "DependencyType"
                }
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "emptyArrayValue",
                Value = new ArrayElement()
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "typedEmptyArrayValue",
                Value = new ArrayElement
                {
                    TypeName = "MyElementType"
                }
            });
            var arrayWithValues = new ArrayElement();
            arrayWithValues.Values.Add(new DependencyElement());
            arrayWithValues.Values.Add(new ValueElement()
            {
                Value = "something"
            });
            arrayWithValues.Values.Add(new DependencyElement()
            {
                Name = "dependencyName"
            });
            ctorElement.Parameters.Add(new ParameterElement()
            {
                Name = "arrayWithValues",
                Value = arrayWithValues
            });

            var registration = new RegisterElement
            {
                TypeName = "someType"
            };
            registration.InjectionMembers.Add(ctorElement);

            var container = new ContainerElement();
            container.Registrations.Add(registration);
            s.Containers.Add(container);
        }

        private static void FillSectionWithConstructors(UnityConfigurationSection s)
        {
            var zeroArgCtor = new ConstructorElement();
            var intCtor = new ConstructorElement();
            intCtor.Parameters.Add(new ParameterElement
            {
                Name = "intParam",
                Value = new ValueElement
                {
                    Value = "23"
                }
            });

            var zeroArgRegistration = new RegisterElement
            {
                TypeName = "SomeType",
                Name = "zeroArg"
            };

            zeroArgRegistration.InjectionMembers.Add(zeroArgCtor);

            var oneArgRegistration = new RegisterElement
            {
                TypeName = "SomeType",
                Name = "oneArg"
            };

            oneArgRegistration.InjectionMembers.Add(intCtor);

            var container = new ContainerElement();
            container.Registrations.Add(zeroArgRegistration);
            container.Registrations.Add(oneArgRegistration);
            s.Containers.Add(container);
        }
    }
}
