using System;
using System.Linq;
using System.Collections.Generic;

namespace Unity
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="TransienLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    public class HierarchicalTransientLifetimeManager : HierarchicalLifetimeManager
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public override void SetValue(object newValue)
        {
            if (newValue is IDisposable disposable)
                disposables.Add(disposable);
        }

        public override object GetValue()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (IDisposable disposable in disposables.Reverse<IDisposable>())
            {
                disposable.Dispose();
            }
            disposables.Clear();
            base.Dispose(disposing);
        }
    }
}
