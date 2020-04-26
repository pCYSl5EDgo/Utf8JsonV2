// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal
{
    public static class RuntimeFeature
    {
        public static bool IsDynamicCodeSupported =>
#if RUNTIME_FEATURE_DYNAMIC_CODE_SUPPORTED
            System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported;
#elif UNITY_2018_4_OR_NEWER && (ENABLE_IL2CPP || NET_STANDARD_2_0)
            false;
#else
            true;
#endif
    }
}
