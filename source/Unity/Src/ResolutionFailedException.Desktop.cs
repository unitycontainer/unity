// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.Practices.Unity
{
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "Implementing serialization with the new transparent approach")]
    [Serializable]
    partial class ResolutionFailedException
    {
        #region Serialization Support

        partial void RegisterSerializationHandler()
        {
            this.SerializeObjectState += (s, e) =>
                {
                    e.AddSerializedState(new ResolutionFailedExceptionSerializationData(this.typeRequested, this.nameRequested));
                };
        }

        [Serializable]
        private struct ResolutionFailedExceptionSerializationData : ISafeSerializationData
        {
            private string typeRequested;
            private string nameRequested;

            public ResolutionFailedExceptionSerializationData(string typeRequested, string nameRequested)
            {
                this.typeRequested = typeRequested;
                this.nameRequested = nameRequested;
            }

            public void CompleteDeserialization(object deserialized)
            {
                var exception = (ResolutionFailedException)deserialized;
                exception.typeRequested = this.typeRequested;
                exception.nameRequested = this.nameRequested;
            }
        }

        #endregion
    }
}
