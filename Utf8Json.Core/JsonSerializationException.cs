// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace Utf8Json
{
    [Serializable]
    public class JsonSerializationException : Exception
    {
        public JsonSerializationException()
        {
        }

        public JsonSerializationException(string message) : base(message)
        {
        }

        public JsonSerializationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected JsonSerializationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}