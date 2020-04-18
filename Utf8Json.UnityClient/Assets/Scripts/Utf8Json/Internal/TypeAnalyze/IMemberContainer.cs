// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public interface IMemberContainer
    {
        Type TargetType { get; }

        ReadOnlySpan<byte> GetPropertyNameRaw();
        ReadOnlySpan<byte> GetPropertyNameWithQuotation();
        ReadOnlySpan<byte> GetPropertyNameWithQuotationAndNameSeparator();
        ReadOnlySpan<byte> GetValueSeparatorAndPropertyNameWithQuotationAndNameSeparator();

#if CSHARP_8_OR_NEWER
        object? GetValue(object @this);
#else
        object GetValue(object @this);
#endif
    }
}
