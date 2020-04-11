// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utf8Json.Internal
{
    public sealed class Lookup<TKey, TElement>
        : ILookup<TKey, TElement>
#if CSHARP_8_OR_NEWER
            where TKey : notnull
#endif
    {
        private readonly Dictionary<TKey, IGrouping<TKey, TElement>> groupings;

        public Lookup(Dictionary<TKey, IGrouping<TKey, TElement>> groupings)
        {
            this.groupings = groupings;
        }

        public IEnumerable<TElement> this[TKey key] => groupings[key];

        public int Count => groupings.Count;

        public bool Contains(TKey key) => groupings.ContainsKey(key);

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() => groupings.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => groupings.Values.GetEnumerator();
    }
}