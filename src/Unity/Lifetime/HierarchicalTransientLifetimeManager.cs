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
        private readonly List<object> objects = new List<object>();

        public override void SetValue(object newValue)
        {
            objects.Add(newValue);
        }

        public override object GetValue()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (IDisposable disposable in objects.Select(o => o as IDisposable)
                                                      .Where(o => null != o)
                                                      .Reverse())
            {
                disposable.Dispose();
            }

            objects.Clear();
            base.Dispose(disposing);
        }
    }
}
