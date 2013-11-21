// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Practices.Unity.Mvc
{
    /// <summary>
    /// Implementation of the <see cref="IHttpModule"/> interface that provides support for using the
    /// <see cref="PerRequestLifetimeManager"/> lifetime manager, and enables it to
    /// dispose the instances after the HTTP request ends.
    /// </summary>
    public class UnityPerRequestHttpModule : IHttpModule
    {
        private static readonly object moduleKey = new object();

        internal static object GetValue(object lifetimeManagerKey)
        {
            var dict = GetDictionary();

            if (dict != null)
            {
                object obj = null;

                if (dict.TryGetValue(lifetimeManagerKey, out obj))
                {
                    return obj;
                }
            }

            return null;
        }

        internal static void SetValue(object lifetimeManagerKey, object value)
        {
            var dict = GetDictionary();

            if (dict == null)
            {
                dict = new Dictionary<object, object>();

                HttpContext.Current.Items[moduleKey] = dict;
            }

            dict[lifetimeManagerKey] = value;
        }

        /// <summary>
        /// Disposes the resources used by this module.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="HttpApplication"/> that provides access to the methods, properties,
        /// and events common to all application objects within an ASP.NET application.</param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += OnEndRequest;
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var dict = GetDictionary();

            if (dict != null)
            {
                foreach (var disposable in dict.Values.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }
        }

        private static Dictionary<object, object> GetDictionary()
        {
            if (HttpContext.Current == null)
                throw new InvalidOperationException();

            var dict = (Dictionary<object, object>)HttpContext.Current.Items[moduleKey];

            return dict;
        }
    }
}
