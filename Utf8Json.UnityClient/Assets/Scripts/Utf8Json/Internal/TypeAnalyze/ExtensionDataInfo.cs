// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

        public readonly bool Add;

#if CSHARP_8_OR_NEWER
        public ExtensionDataInfo(PropertyInfo? info)
#else
        public ExtensionDataInfo(PropertyInfo info)
#endif
        {
            Info = info;
            if (info is null)
            {
                Add = false;
            }
            else
            {
                if (info.GetMethod is null)
                {
                    throw new InvalidOperationException();
                }

                Add = info.SetMethod is null;
            }
        }
    }
}
