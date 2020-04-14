// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Internal
{
    public static class FromTypeToMethodHandles
    {
        public static ThreadSafeTypeKeyFormatterHashTable.Entry GetEntry<T, TFormatter>()
        {
            var formatterType = typeof(TFormatter);
            return new ThreadSafeTypeKeyFormatterHashTable.Entry(typeof(T),
                StaticHelper.GetSerializeStatic(formatterType),
                StaticHelper.GetDeserializeStatic(formatterType),
                StaticHelper.GetCalcByteLengthForSerialization(formatterType),
                StaticHelper.GetSerializeSpan(formatterType)
                );
        }
    }
}