// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.Practices.Unity
{
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "Implementing serialization with the new transparent approach")]
    [Serializable]
    partial class DuplicateTypeMappingException
    {
        #region Serialization Support

        partial void RegisterSerializationHandler()
        {
            this.SerializeObjectState += (s, e) =>
            {
                e.AddSerializedState(new DuplicateTypeMappingExceptionSerializationData(this.name, this.mappedFromType, this.currentMappedToType, this.newMappedToType));
            };
        }

        [Serializable]
        private struct DuplicateTypeMappingExceptionSerializationData : ISafeSerializationData
        {
            private string name;
            private string mappedFromType;
            private string currentMappedToType;
            private string newMappedToType;

            public DuplicateTypeMappingExceptionSerializationData(string name, string mappedFromType, string currentMappedToType, string newMappedToType)
            {
                this.name = name;
                this.mappedFromType = mappedFromType;
                this.currentMappedToType = currentMappedToType;
                this.newMappedToType = newMappedToType;
            }

            public void CompleteDeserialization(object deserialized)
            {
                var exception = (DuplicateTypeMappingException)deserialized;
                exception.name = this.name;
                exception.mappedFromType = this.mappedFromType;
                exception.currentMappedToType = this.currentMappedToType;
                exception.newMappedToType = this.newMappedToType;
            }
        }

        #endregion
    }
}
