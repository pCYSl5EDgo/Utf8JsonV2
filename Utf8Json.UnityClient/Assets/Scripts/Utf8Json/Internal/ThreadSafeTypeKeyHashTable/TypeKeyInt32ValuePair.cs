// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Copyright (c) pCYSl5EDgo. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public readonly struct TypeKeyInt32ValuePair
    {
        public readonly Type Key;
        public readonly int Value;

        public TypeKeyInt32ValuePair(Type key, int value)
        {
            Key = key;
            Value = value;
        }

        public override int GetHashCode() => Key.GetHashCode();
    }
}
