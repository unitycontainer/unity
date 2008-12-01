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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    public partial class BuildFailedExceptionTest
    {
        [TestMethod]
        public void BuildFailedExceptionsSerializeProperly()
        {
            BuildFailedException ex = new BuildFailedException(new ThrowingStrategy(""), 3,
                NamedTypeBuildKey.Make<object>(), null);

            BinaryFormatter formatter = new BinaryFormatter();
            byte[] serializedBytes;
            using(MemoryStream serializingStream = new MemoryStream())
            {
                formatter.Serialize(serializingStream, ex);
                serializedBytes = serializingStream.ToArray();
            }

            BuildFailedException deserializedException;
            using (MemoryStream deserializingStream = new MemoryStream(serializedBytes))
            {
                deserializedException = (BuildFailedException) formatter.Deserialize(deserializingStream);
            }

            StringAssert.Contains(deserializedException.Message, "ThrowingStrategy");
        }
    }
}
