// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal
{
    public static class FromTypeToMethodHandles
    {
        public static ThreadSafeTypeKeyFormatterHashTable.Entry GetEntry<TTarget, TFormatter>()
        {
            var formatterType = typeof(TFormatter);
            var targetType = typeof(TTarget);
            return new ThreadSafeTypeKeyFormatterHashTable.Entry(targetType,
                StaticHelper.GetSerializeStatic(formatterType),
                StaticHelper.GetDeserializeStatic(formatterType),
                StaticHelper.GetCalcByteLengthForSerialization(formatterType),
                StaticHelper.GetSerializeSpan(formatterType)
                );
        }
    }
}
