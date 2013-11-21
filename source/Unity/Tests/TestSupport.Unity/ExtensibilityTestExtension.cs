// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
