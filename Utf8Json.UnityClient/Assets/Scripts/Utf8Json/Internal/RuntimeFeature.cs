// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal
{
    public static class RuntimeFeature
    {
        public static bool IsDynamicCodeSupported =>
#if RUNTIME_FEATURE_DYNAMIC_CODE_SUPPORTED
            System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported;
#elif ENABLE_IL2CPP
            false;
#else
            true;
#endif
    }
}
