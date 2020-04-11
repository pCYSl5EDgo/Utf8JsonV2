// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace Utf8Json.Internal
{
    internal static class StringEncoding
    {
        public static readonly Encoding Utf8 = new UTF8Encoding(false);
    }
}