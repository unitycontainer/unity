// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using Unity.Mvc.Properties;
using Unity.Utility;

namespace Unity.Mvc
{
    /// <summary>
    /// Implementation of the <see cref="IHttpModule"/> interface that provides support for using the
    /// <see cref="PerRequestLifetimeManager"/> lifetime manager, and enables it to
    /// dispose the instances after the HTTP request ends.
    /// </summary>
    public class UnityPerRequestHttpModule : IHttpModule
    {
        private static readonly object ModuleKey = new object();

        internal static object GetValue(object lifetimeManagerKey)
        {
            var dict = GetDictionary(HttpContext.Current);

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
            var dict = GetDictionary(HttpContext.Current);

            if (dict == null)
            {
                dict = new Dictionary<object, object>();

                HttpContext.Current.Items[ModuleKey] = dict;
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated with Guard class")]
        public void Init(HttpApplication context)
        {
            Guard.ArgumentNotNull(context, "context");
            context.EndRequest += OnEndRequest;
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            var dict = GetDictionary(app.Context);

            if (dict != null)
            {
                foreach (var disposable in dict.Values.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }
        }

        private static Dictionary<object, object> GetDictionary(HttpContext context)
        {
            if (context == null)
            {
                throw new InvalidOperationException(Resources.ErrorHttpContextNotAvailable);
            }

            var dict = (Dictionary<object, object>)context.Items[ModuleKey];

            return dict;
        }
    }
}
