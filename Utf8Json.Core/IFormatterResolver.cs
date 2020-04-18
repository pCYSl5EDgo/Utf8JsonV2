// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json
{
    public interface IFormatterResolver
    {
#if CSHARP_8_OR_NEWER
        IJsonFormatter<T>? GetFormatter<T>();
        IJsonFormatter? GetFormatter(Type targetType);
#else
        IJsonFormatter<T> GetFormatter<T>();
        IJsonFormatter GetFormatter(Type targetType);
#endif
        IntPtr GetSerializeStatic<T>();
        IntPtr GetDeserializeStatic<T>();
        IntPtr GetCalcByteLengthForSerialization<T>();
        IntPtr GetSerializeSpan<T>();
    }
}
