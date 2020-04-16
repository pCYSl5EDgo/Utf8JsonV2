// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Utf8Json.Generator
{
    public interface IMemberSerializationInfo
    {
        int? ConstructorParameterIndex { get; }

        bool IsField { get; }
    }
}
