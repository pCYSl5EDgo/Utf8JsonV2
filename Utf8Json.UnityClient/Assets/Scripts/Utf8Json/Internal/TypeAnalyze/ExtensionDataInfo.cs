// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utf8Json.Internal
{
    public readonly struct ExtensionDataInfo
    {
#if CSHARP_8_OR_NEWER
        public readonly PropertyInfo? Info;
#else
        public readonly PropertyInfo Info;
#endif
        public readonly ExtensionDataKind Kind;

        public Type TargetType => Info?.PropertyType ?? typeof(Dictionary<string, object>);

        public ExtensionDataInfo(PropertyInfo info)
        {
            Info = info;
            Kind = info.PropertyType == typeof(Dictionary<string, object>) ? ExtensionDataKind.Object : ExtensionDataKind.JsonElement;
        }

#if CSHARP_8_OR_NEWER
        public Dictionary<string, object?>? GetValue(object @this)
        {
            return Info?.GetValue(@this) as Dictionary<string, object?>;
        }
#else
        public Dictionary<string, object> GetValue(object @this)
        {
            return Info?.GetValue(@this) as Dictionary<string, object>;
        }
#endif
    }
}
