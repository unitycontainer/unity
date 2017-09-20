// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Override
{
    public class TestTypeInConfig
    {
        public TestTypeInConfig(int value)
        {
            Value = value;
        }

        public TestTypeInConfig()
        {
            Value = 1;
        }

        public int Value { get; set; }
        public int X { get; set; }
        public string Y { get; set; }
    }
}