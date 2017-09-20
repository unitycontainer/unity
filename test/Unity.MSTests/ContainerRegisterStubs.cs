// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unity.Container.Register.Tests.Stubs
{
    public class TypeConver : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Temp);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Temp();
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                int intValue = (int)value;
                return "Converted Temp";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class MyClass : MyAbsClass
    {
        public object AbsProp1 { get; set; }
    }

    public abstract class MyAbsClass
    {
        [Unity.Dependency]
        private object AbsProp1 { get; set; }
    }

    public class MyDerClass : myBaseClass
    {
        public object BaseProp1 { get; set; }
    }

    public class myBaseClass
    {
        [Unity.Dependency]
        private object BaseProp1 { get; set; }
    }

    public class MyDependency
    {
        private object myDepObj;

        public MyDependency(object obj)
        {
            this.myDepObj = obj;
        }
    }

    public class MyDependency1
    {
        private object myDepObj;

        public MyDependency1(object obj)
        {
            this.myDepObj = obj;
        }

        [Unity.InjectionConstructor]
        public MyDependency1(string str)
        {
            this.myDepObj = str;
        }

        public object MyDepObj
        {
            get { return myDepObj; }
        }
    }

    public class MultipleConstructors
    {
        public MultipleConstructors()
        {
            Console.WriteLine("Default Empty constructor");
        }

        public MultipleConstructors(object obj)
        {
            Console.WriteLine("object constructor");
        }
    }

    public class MySetterInjectionClass
    {
        private object myObj;

        public object MyObj
        {
            get { return myObj; }
            set { myObj = value; }
        }
    }

    public class MySetterDependencyClass
    {
        [Unity.Dependency]
        public object MyObj { get; set; }
    }

    public class MySetterDependencyNonDependencyClass
    {
        [Dependency]
        public object MyObj { get; set; }

        public object MyAnotherObj { get; set; }
    }

    public class My2PropertyDependencyClass
    {
        [Dependency]
        public object MyFirstObj { get; set; }

        [Dependency]
        public object MySecondObj { get; set; }
    }

    public class MyTypeFirst
    {
        private string myObj;

        public MyTypeFirst()
        {
            myObj = "MyTypeFirst";
        }

        public string MyObj
        {
            get { return myObj; }
        }
    }

    public class MyTypeSecond
    {
        public MyTypeSecond()
        {
        }

        public string MyObj { get; set; }
    }

    public class MyMethodDependencyClass
    {
        private string myObj;

        [Unity.InjectionMethod]
        public void Initialize(string obj)
        {
            myObj = obj;
        }

        public object MyObj
        {
            get { return myObj; }
        }
    }

    internal interface IMySingeltonInterface
    {
    }

    internal class MyFirstSingetonclass : IMySingeltonInterface
    {
    }

    internal class MySecondSingetonclass : IMySingeltonInterface
    {
    }

    internal interface IMyClass
    {
    }

    internal class MyBaseClass : IMyClass
    {
    }

    internal class MyClassDerivedBaseClass : MyBaseClass
    {
    }

    internal class MyDisposeClass : IDisposable
    {
        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    internal class MixedClass : IMyInterface, ITemporary
    {
    }

    internal interface IMyInterface
    {
    }

    internal interface ITemporary
    {
    }

    internal class Temp : ITemporary
    {
    }

    internal class Temp2 : ITemporary
    {
    }

    internal class Temporary : ITemporary
    {
    }

    internal class TempGeneric<K> : ITemporary
    {
        [InjectionConstructor]
        public TempGeneric(TempGeneric2<K> t1)
        {
            string t = string.Empty;
            if (typeof(K) == typeof(String))
            {
                t = "string";
            }
            else
            {
                t = "int";
            }
        }
    }

    internal class TempGeneric2<T> : ITemporary
    {
        [InjectionConstructor]
        public TempGeneric2()
        {
            string t = string.Empty;
            if (typeof(T) == typeof(String))
            {
                t = "string";
            }
            else
            {
                t = "int";
            }
        }
    }
}
