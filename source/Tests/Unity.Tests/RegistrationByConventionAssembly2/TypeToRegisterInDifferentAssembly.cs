// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using RegistrationByConventionAssembly1;

namespace RegistrationByConventionAssembly2
{
    public class TypeToRegisterInDifferentAssembly : ITypeImplementingI1
    {
        public void MethodInI1()
        {
            throw new NotImplementedException();
        }
    }
}
