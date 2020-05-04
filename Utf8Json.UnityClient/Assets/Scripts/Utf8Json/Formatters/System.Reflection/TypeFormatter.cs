// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace Utf8Json.Formatters
{
    public sealed class TypeFormatter
#if CSHARP_8_OR_NEWER
        : IJsonFormatter<Type?>
#else
        : IJsonFormatter<Type>
#endif
    {
        public static bool ThrowOnError = true;
        public static bool IgnoreCase = false;
        public static bool SerializeAssemblyQualifiedName = true;
        public static bool DeserializeSubtractAssemblyQualifiedName = true;

        private static readonly Regex subtractFullNameRegex =
#if ENABLE_IL2CPP
            new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+");
#else
            new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);
#endif

#if CSHARP_8_OR_NEWER
        public void Serialize(ref JsonWriter writer, Type? value, JsonSerializerOptions options)
#else
        public void Serialize(ref JsonWriter writer, Type value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value, options);
        }

#if CSHARP_8_OR_NEWER
        public static void SerializeStatic(ref JsonWriter writer, Type? value, JsonSerializerOptions options)
#else
        public static void SerializeStatic(ref JsonWriter writer, Type value, JsonSerializerOptions options)
#endif
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.Write((SerializeAssemblyQualifiedName ? value.AssemblyQualifiedName : value.FullName) ?? throw new NullReferenceException());
        }

#if CSHARP_8_OR_NEWER
        public Type? Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#else
        public Type Deserialize(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }

#if CSHARP_8_OR_NEWER
        public static Type? DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#else
        public static Type DeserializeStatic(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var typeName = reader.ReadString();
            if (DeserializeSubtractAssemblyQualifiedName)
            {
                typeName = subtractFullNameRegex.Replace(typeName, "");
            }

            return Type.GetType(typeName ?? throw new NullReferenceException(), ThrowOnError, IgnoreCase);
        }


#if CSHARP_8_OR_NEWER
        public void SerializeTypeless(ref JsonWriter writer, object? value, JsonSerializerOptions options)
#else
        public void SerializeTypeless(ref JsonWriter writer, object value, JsonSerializerOptions options)
#endif
        {
            SerializeStatic(ref writer, value as Type, options);
        }

#if CSHARP_8_OR_NEWER
        public object? DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#else
        public object DeserializeTypeless(ref JsonReader reader, JsonSerializerOptions options)
#endif
        {
            return DeserializeStatic(ref reader, options);
        }
    }
}
