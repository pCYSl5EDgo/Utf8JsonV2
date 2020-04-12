// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Emit
{
    internal class MetaMember
    {
        public string Name { get; private set; }
        public string MemberName { get; private set; }

        public bool IsProperty => PropertyInfo != null;
        public bool IsField => FieldInfo != null;
        public bool IsWritable { get; private set; }
        public bool IsReadable { get; private set; }
        public Type Type { get; private set; }
#if CSHARP_8_OR_NEWER
        public FieldInfo? FieldInfo { get; private set; }
        public PropertyInfo? PropertyInfo { get; private set; }
        public MethodInfo? ShouldSerializeMethodInfo { get; private set; }

        private readonly MethodInfo? getMethod;
        private readonly MethodInfo? setMethod;
#else
        public FieldInfo FieldInfo { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public MethodInfo ShouldSerializeMethodInfo { get; private set; }

        private readonly MethodInfo getMethod;
        private readonly MethodInfo setMethod;
#endif


        protected MetaMember(Type type, string name, string memberName, bool isWritable, bool isReadable)
        {
            this.Name = name;
            this.MemberName = memberName;
            this.Type = type;
            this.IsWritable = isWritable;
            this.IsReadable = isReadable;
        }

        public MetaMember(FieldInfo info, string name, bool allowPrivate)
        {
            this.Name = name;
            this.MemberName = info.Name;
            this.FieldInfo = info;
            this.Type = info.FieldType;
            this.IsReadable = allowPrivate || info.IsPublic;
            this.IsWritable = allowPrivate || (info.IsPublic && !info.IsInitOnly);
            this.ShouldSerializeMethodInfo = GetShouldSerialize(info);
        }

        public MetaMember(PropertyInfo info, string name, bool allowPrivate)
        {
            this.getMethod = info.GetGetMethod(true);
            this.setMethod = info.GetSetMethod(true);

            this.Name = name;
            this.MemberName = info.Name;
            this.PropertyInfo = info;
            this.Type = info.PropertyType;
            this.IsReadable = (getMethod != null) && (allowPrivate || getMethod.IsPublic) && !getMethod.IsStatic;
            this.IsWritable = (setMethod != null) && (allowPrivate || setMethod.IsPublic) && !setMethod.IsStatic;
            this.ShouldSerializeMethodInfo = GetShouldSerialize(info);
        }

        private static MethodInfo GetShouldSerialize(MemberInfo info)
        {
            var shouldSerialize = "ShouldSerialize" + info.Name;

            // public only
            Debug.Assert(info.DeclaringType != null, "info.DeclaringType != null");
            return info.DeclaringType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == shouldSerialize && x.ReturnType == typeof(bool) && x.GetParameters().Length == 0);
        }

#if CSHARP_8_OR_NEWER
        public T? GetCustomAttribute<T>(bool inherit) where T : Attribute
#else
        public T GetCustomAttribute<T>(bool inherit) where T : Attribute
#endif
        {
            return IsProperty ? PropertyInfo?.GetCustomAttribute<T>(inherit) : FieldInfo?.GetCustomAttribute<T>(inherit);
        }

        public virtual void EmitLoadValue(ILGenerator il)
        {
            if (IsProperty)
            {
                Debug.Assert(getMethod != null, nameof(getMethod) + " != null");
                il.EmitCall(getMethod);
            }
            else
            {
                Debug.Assert(FieldInfo != null, nameof(FieldInfo) + " != null");
                il.Emit(OpCodes.Ldfld, FieldInfo);
            }
        }

        public virtual void EmitStoreValue(ILGenerator il)
        {
            if (IsProperty)
            {
                Debug.Assert(setMethod != null, nameof(setMethod) + " != null");
                il.EmitCall(setMethod);
            }
            else
            {
                Debug.Assert(FieldInfo != null, nameof(FieldInfo) + " != null");
                il.Emit(OpCodes.Stfld, FieldInfo);
            }
        }
    }

    // used for serialize exception...
    internal class StringConstantValueMetaMember : MetaMember
    {
        private readonly string constant;

        public StringConstantValueMetaMember(string name, string constant)
            : base(typeof(string), name, name, false, true)
        {
            this.constant = constant;
        }

        public override void EmitLoadValue(ILGenerator il)
        {
            il.Emit(OpCodes.Pop); // pop load instance
            il.Emit(OpCodes.Ldstr, constant);
        }

        public override void EmitStoreValue(ILGenerator il)
        {
            throw new NotSupportedException();
        }
    }

    // used for serialize exception...
    internal class InnerExceptionMetaMember : MetaMember
    {
        private static readonly MethodInfo getInnerException;
        private static readonly MethodInfo nonGenericSerialize;

        // set after...
        internal ArgumentField ArgWriter;
        internal ArgumentField ArgValue;
        internal ArgumentField ArgOptions;

        public InnerExceptionMetaMember(string name)
            : base(typeof(Exception), name, name, false, true)
        {
        }

        static InnerExceptionMetaMember()
        {
            var _0 = ExpressionUtility.GetPropertyInfo((Exception ex) => ex.InnerException).GetGetMethod();
            Debug.Assert(_0 != null, nameof(getInnerException) + " != null");
            getInnerException = _0;
            var jsonSerializer = Type.GetType("Utf8Json.JsonSerializer");
            // ReSharper disable once ConstantConditionalAccessQualifier
            var _1 = jsonSerializer?.GetMethods()?.FirstOrDefault(PredicateNonGenericSerialize)?.MakeGenericMethod(typeof(object));
            Debug.Assert(_1 != null, nameof(nonGenericSerialize) + " != null");
            nonGenericSerialize = _1;
        }

        private static bool PredicateNonGenericSerialize(MethodInfo x)
        {
            if (x.IsStatic || x.Name != "Serialize")
            {
                return false;
            }

            var parameters = x.GetParameters();
            return parameters.Length == 3 && parameters[0].ParameterType.IsByRef;
        }

        public override void EmitLoadValue(ILGenerator il)
        {
            il.Emit(OpCodes.Callvirt, getInnerException);
        }

        public override void EmitStoreValue(ILGenerator il)
        {
            throw new NotSupportedException();
        }

        public void EmitSerializeDirectly(ILGenerator il)
        {
            // JsonSerializer.NonGeneric.Serialize(ref writer, value.InnerException, formatterResolver);
            ArgWriter.EmitLoad();
            ArgValue.EmitLoad();
            il.Emit(OpCodes.Callvirt, getInnerException);
            ArgOptions.EmitLoad();
            il.EmitCall(nonGenericSerialize);
        }
    }
}
#endif
