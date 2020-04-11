// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json.Internal
{
    public class JsonParsingException : Exception
    {
        public JsonParsingException(string message)
            : base(message)
        {
        }
    }
}