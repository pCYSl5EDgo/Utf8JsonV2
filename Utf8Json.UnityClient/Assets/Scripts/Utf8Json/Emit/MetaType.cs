// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using Utf8Json.Internal;

namespace Utf8Json.Emit
{
    internal class MetaType
    {
        public Type Type { get; private set; }
        public bool IsClass { get; private set; }
        public bool IsStruct => !IsClass;
        public bool IsConcreteClass { get; private set; }
#if CSHARP_8_OR_NEWER
        public ConstructorInfo? BestmatchConstructor { get; internal set; }
#else
        public ConstructorInfo BestmatchConstructor { get; internal set; }
#endif
        public MetaMember[] ConstructorParameters { get; internal set; }
        public MetaMember[] Members { get; internal set; }

        public MetaType(Type type, Func<string, string> nameMutator, bool allowPrivate)
        {
            var ti = type.GetTypeInfo();
            var isClass = ti.IsClass || ti.IsInterface || ti.IsAbstract;

            this.Type = type;
            var stringMembers = new Dictionary<string, MetaMember>();

            {
                foreach (var item in type.GetAllProperties())
                {
                    if (item.GetIndexParameters().Length > 0) continue; // skip indexer
                    if (item.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null) continue;

                    var dm = item.GetCustomAttribute<DataMemberAttribute>(true);
                    var name = (dm != null && dm.Name != null) ? dm.Name : nameMutator(item.Name);

                    var member = new MetaMember(item, name, allowPrivate);
                    if (!member.IsReadable && !member.IsWritable) continue;

                    if (stringMembers.ContainsKey(member.Name))
                    {
                        throw new InvalidOperationException("same (custom)name is in type. Type:" + type.Name + " Name:" + member.Name);
                    }
                    stringMembers.Add(member.Name, member);
                }
                foreach (var item in type.GetAllFields())
                {
                    if (item.GetCustomAttribute<IgnoreDataMemberAttribute>(true) != null) continue;
                    if (item.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true) != null) continue;
                    if (item.IsStatic) continue;
                    if (item.Name.StartsWith("<")) continue; // compiler generated field(anonymous type, etc...)

                    var dm = item.GetCustomAttribute<DataMemberAttribute>(true);
                    var name = (dm != null && dm.Name != null) ? dm.Name : nameMutator(item.Name);

                    var member = new MetaMember(item, name, allowPrivate);
                    if (!member.IsReadable && !member.IsWritable) continue;

                    if (stringMembers.ContainsKey(member.Name))
                    {
                        throw new InvalidOperationException("same (custom)name is in type. Type:" + type.Name + " Name:" + member.Name);
                    }
                    stringMembers.Add(member.Name, member);
                }
            }

            // GetConstructor
#if CSHARP_8_OR_NEWER
            ConstructorInfo? ctor =
#else
            var ctor =
#endif
                ti.DeclaredConstructors.Where(x => x.IsPublic)
                    .SingleOrDefault(x => x.GetCustomAttribute<SerializationConstructorAttribute>(false) != null);
            var constructorParameters = new List<MetaMember>();
            {
                var ctorEnumerator = default(IEnumerator<ConstructorInfo>);
                if (ctor == null)
                {
                    // descending.
                    ctorEnumerator = ti.DeclaredConstructors.Where(x => x.IsPublic).OrderByDescending(x => x.GetParameters().Length).GetEnumerator();
                    if (ctorEnumerator.MoveNext())
                    {
                        ctor = ctorEnumerator.Current;
                    }
                }

                if (ctor != null)
                {
                    var constructorLookupDictionary = stringMembers.ToLookup(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);
                    do
                    {
                        constructorParameters.Clear();
                        Debug.Assert(ctor != null, nameof(ctor) + " != null");
                        foreach (var item in ctor.GetParameters())
                        {
                            var hasKey = constructorLookupDictionary[item.Name];
                            var hasKeyEnumerator = hasKey.GetEnumerator();
                            try
                            {
                                if (!hasKeyEnumerator.MoveNext())
                                {
                                    ctor = null;
                                    continue;
                                }

                                var paramMember = hasKeyEnumerator.Current.Value;

                                if (hasKeyEnumerator.MoveNext())
                                {
                                    if (ctorEnumerator == null)
                                    {
                                        throw new InvalidOperationException("duplicate matched constructor parameter name:" + type.FullName + " parameterName:" + item.Name + " paramterType:" + item.ParameterType.Name);
                                    }

                                    ctor = null;
                                    continue;
                                }

                                if (item.ParameterType == paramMember.Type && paramMember.IsReadable)
                                {
                                    constructorParameters.Add(paramMember);
                                }
                                else
                                {
                                    ctor = null;
                                }
                            }
                            finally
                            {
                                hasKeyEnumerator.Dispose();
                            }
                        }
                    } while (TryGetNextConstructor(ctorEnumerator, ref ctor));
                }
            }

            this.IsClass = isClass;
            this.IsConcreteClass = isClass && !(ti.IsAbstract || ti.IsInterface);
            this.BestmatchConstructor = ctor;
            this.ConstructorParameters = constructorParameters.ToArray();
            this.Members = stringMembers.Values.ToArray();
        }
#if CSHARP_8_OR_NEWER
        private static bool TryGetNextConstructor(IEnumerator<ConstructorInfo> ctorEnumerator, ref ConstructorInfo? ctor)
#else
        private static bool TryGetNextConstructor(IEnumerator<ConstructorInfo> ctorEnumerator, ref ConstructorInfo ctor)
#endif
        {
            if (ctorEnumerator == null || ctor != null)
            {
                return false;
            }

            if (ctorEnumerator.MoveNext())
            {
                ctor = ctorEnumerator.Current;
                return true;
            }
            ctor = default;
            return false;
        }
    }
}
#endif
