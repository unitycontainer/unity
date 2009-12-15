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

namespace Microsoft.Practices.Unity.TestSupport
{
    public interface IConfigOne : IUnityContainerExtensionConfigurator
    {
        IConfigOne SetText(string text);
    }

    public interface IConfigTwo : IUnityContainerExtensionConfigurator
    {
        IConfigTwo SetMessage(string text);
    }

    public class ExtensibilityTestExtension : UnityContainerExtension, IConfigOne, IConfigTwo
    {
        public string ConfigOneText { get; private set; }
        public string ConfigTwoText { get; private set; }

        protected override void Initialize()
        {
            
        }

        public IConfigOne SetText(string text)
        {
            ConfigOneText = text;
            return this;
        }

        public IConfigTwo SetMessage(string text)
        {
            ConfigTwoText = text;
            return this;
        }
    }
}
