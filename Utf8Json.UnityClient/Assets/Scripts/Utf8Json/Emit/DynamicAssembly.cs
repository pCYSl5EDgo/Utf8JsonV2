// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Emit
{
    internal class DynamicAssembly
    {
        private readonly ModuleBuilder moduleBuilder;

        // don't expose ModuleBuilder
        // public ModuleBuilder ModuleBuilder { get { return moduleBuilder; } }

        private readonly object gate = new object();

        // requires lock on mono environment. see: https://github.com/neuecc/MessagePack-CSharp/issues/161

        public TypeBuilder DefineType(string name, TypeAttributes attr)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr, parent);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr, parent, interfaces);
            }
        }

        public DynamicAssembly(string moduleName)
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.Run);
            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
        }
    }
}
#endif
