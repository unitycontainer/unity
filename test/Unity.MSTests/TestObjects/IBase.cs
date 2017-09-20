// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Tests.TestObjects
{
    public interface IBase
    {
        IService Service { get; set; }
    }

    public interface ILazyDependency
    {
        Lazy<EmailService> Service { get; set; }
    }

    public class Base : IBase
    {
        [Dependency]
        public IService Service { get; set; }
    }

    public class LazyDependency : ILazyDependency
    {
        [Dependency]
        public Lazy<EmailService> Service { get; set; }
    }

    public class LazyDependencyConstructor
    {
        private Lazy<EmailService> service = null;
        
        public LazyDependencyConstructor(Lazy<EmailService> s)
        {
            service = s;
        }
    }
}