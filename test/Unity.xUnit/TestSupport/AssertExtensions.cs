// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Xunit;

namespace Unity.TestSupport
{
    public static class AssertExtensions
    {
        public static void AssertException<TException>(Action action)
            where TException : Exception
        {
            AssertException<TException>(action, (e) => { });
        }

        public static void AssertException<TException>(Action action, Action<TException> callback)
           where TException : Exception
        {
            try
            {
                action();
                Assert.True(false, String.Format("Expected exception of type {0}", typeof(TException).GetTypeInfo().Name));
            }
            catch (TException e)
            {
                callback(e);
            }
        }

        public static void IsInstanceOfType(object value, Type expectedType)
        {
            Assert.NotNull(value);
            Assert.NotNull(expectedType);
            Assert.True(expectedType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()));
        }
    }
}
