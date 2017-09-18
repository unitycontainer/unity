// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ObjectBuilder2;

namespace Unity
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    public class HierarchicalLifetimeManager : ContainerControlledLifetimeManager
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public override void SetValue(object newValue)
        {
            var disposable = newValue as IDisposable;

            if (null != disposable)
                disposables.Add(disposable);
        }

        public override object GetValue()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }
            disposables.Clear();

            base.Dispose(disposing);
        }
    }
}
