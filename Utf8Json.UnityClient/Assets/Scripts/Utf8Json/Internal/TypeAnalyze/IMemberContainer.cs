// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public interface IMemberContainer
    {
        DirectTypeEnum IsFormatterDirect { get; }
        Type TargetType { get; }

        string MemberName { get; }

#if CSHARP_8_OR_NEWER
        JsonFormatterAttribute? FormatterInfo { get; }
#else
        JsonFormatterAttribute FormatterInfo { get; }
#endif
    }

    public interface IShouldSerializeMemberContainer : IMemberContainer
    {
        MethodInfo ShouldSerialize { get; }
    }
}
