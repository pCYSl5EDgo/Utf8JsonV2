// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Utf8Json
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Gets or sets the default set of options to use when not explicitly specified for a method call.
        /// </summary>
        /// <value>The default value is Utf8Json.Resolvers.Standard.</value>
        /// <remarks>
        /// This is an AppDomain or process-wide setting.
        /// If you're writing a library, you should NOT set or rely on this property but should instead pass
        /// in StandardResolver (or the required options) explicitly to every method call
        /// to guarantee appropriate behavior in any application.
        /// If you are an app author, realize that setting this property impacts the entire application so it should only be
        /// set once, and before any use of <see cref="JsonSerializer"/> occurs.
        /// </remarks>
        public static JsonSerializerOptions DefaultOptions { get; set; }
            = Type.GetType("Utf8Json.Resolvers.StandardResolver")
                ?.GetField("Options", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                ?.GetValue(default) as JsonSerializerOptions
                ?? throw new NullReferenceException();
    }
}
