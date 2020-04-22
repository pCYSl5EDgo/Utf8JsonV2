// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Utf8Json
{
    public sealed class JsonSerializerOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerOptions"/> class.
        /// </summary>
        internal JsonSerializerOptions(IFormatterResolver resolver)
        {
            this.Resolver = resolver;
            this.IgnoreNullValues = true;
            this.IgnoreReadOnlyProperties = false;
            this.MaxDepth = 64;
            this.WriteIndented = false;
            this.IgnoreCase = false;
            this.ProcessEnumAsString = true;
            this.UseCultureInfo = false;
            this.CultureInfo = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerOptions"/> class
        /// with members initialized from an existing instance.
        /// </summary>
        /// <param name="copyFrom">The options to copy from.</param>
        private JsonSerializerOptions(JsonSerializerOptions copyFrom)
        {
            this.Resolver = copyFrom.Resolver;
            this.IgnoreNullValues = copyFrom.IgnoreNullValues;
            this.IgnoreReadOnlyProperties = copyFrom.IgnoreReadOnlyProperties;
            this.MaxDepth = copyFrom.MaxDepth;
            this.WriteIndented = copyFrom.WriteIndented;
            this.IgnoreCase = copyFrom.IgnoreCase;
            this.ProcessEnumAsString = copyFrom.ProcessEnumAsString;
            this.UseCultureInfo = copyFrom.UseCultureInfo;
            this.CultureInfo = copyFrom.CultureInfo;
        }

        /// <summary>
        /// Gets the resolver to use for complex types.
        /// </summary>
        /// <value>An instance of <see cref="IFormatterResolver"/>. Never <c>null</c>.</value>
        /// <exception cref="ArgumentNullException">Thrown if an attempt is made to set this property to <c>null</c>.</exception>
        public IFormatterResolver Resolver { get; private set; }

        /// <summary>
        /// A collection of known dangerous types that are not expected in a typical Json stream,
        /// and thus are rejected by the default implementation of <see cref="ThrowIfDeserializingTypeIsDisallowed(Type)"/>.
        /// </summary>
        private static readonly HashSet<string> blacklistCheck = new HashSet<string>
        {
            "System.CodeDom.Compiler.TempFileCollection",
            "System.Management.IWbemClassObjectFreeThreaded",
        };

        /// <summary>
        /// Checks whether a given type may be deserialized.
        /// </summary>
        /// <param name="type">The type to be instantiated.</param>
        /// <exception cref="TypeAccessException">Thrown if the <paramref name="type"/> is not allowed to be deserialized.</exception>
        /// <remarks>
        /// This method provides a means for an important security mitigation when using the Typeless formatter to prevent untrusted messagepack from
        /// deserializing objects that may be harmful if instantiated, disposed or finalized.
        /// The default implementation throws for only a few known dangerous types.
        /// Applications that deserialize from untrusted sources should override this method and throw if the type is not among the expected set.
        /// </remarks>
        public void ThrowIfDeserializingTypeIsDisallowed(Type type)
        {
            if (blacklistCheck.Contains(type.FullName ?? string.Empty))
            {
                throw new JsonSerializationException("Deserialization attempted to create the type " + type.FullName + " which is not allowed.");
            }
        }

        public bool UseCultureInfo { get; private set; }

        public JsonSerializerOptions WithUseCultureInfo(bool value)
        {
            if (this.UseCultureInfo == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                UseCultureInfo = value,
            };
            return result;
        }

        public CultureInfo CultureInfo { get; private set; }

        public JsonSerializerOptions WithCultureInfo(CultureInfo value)
        {
            if (ReferenceEquals(this.CultureInfo, value))
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                CultureInfo = value,
            };
            return result;
        }

        public bool ProcessEnumAsString { get; private set; }

        public JsonSerializerOptions WithProcessEnumAsString(bool value)
        {
            if (this.ProcessEnumAsString == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                ProcessEnumAsString = value,
            };
            return result;
        }

        public bool IgnoreCase { get; private set; }

        public JsonSerializerOptions WithIgnoreCase(bool value)
        {
            if (this.IgnoreCase == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                IgnoreCase = value,
            };
            return result;
        }

        /// <summary>
        /// Gets a copy of these options with the <see cref="Resolver"/> property set to a new value.
        /// </summary>
        /// <param name="value">The new value for the <see cref="Resolver"/>.</param>
        /// <returns>The new instance; or the original if the value is unchanged.</returns>
        public JsonSerializerOptions WithResolver(IFormatterResolver value)
        {
            if (this.Resolver == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                Resolver = value,
            };
            return result;
        }

        /// <summary>
        /// Get a value that indicates whether an extra comma at the end of a list of JSON values in an object or array is allowed (and ignored) within the JSON payload being deserialized.
        /// Always false due to the RFC.
        /// </summary>
        public bool AllowTrailingCommas => false;

        public JsonSerializerOptions WithAllowTrailingCommas(bool value) => throw new NotSupportedException();

        public bool IgnoreNullValues { get; private set; }

        public JsonSerializerOptions WithIgnoreNullValues(bool value)
        {
            if (this.IgnoreNullValues == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                IgnoreNullValues = value,
            };
            return result;
        }

        public bool IgnoreReadOnlyProperties { get; private set; }

        public JsonSerializerOptions WithIgnoreReadOnlyProperties(bool value)
        {
            if (this.IgnoreReadOnlyProperties == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                IgnoreReadOnlyProperties = value,
            };
            return result;
        }

        public uint MaxDepth { get; private set; }

        public JsonSerializerOptions WithMaxDepth(uint value)
        {
            if (value > 64)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (value == 0)
            {
                value = 64;
            }

            if (this.MaxDepth == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                MaxDepth = value,
            };
            return result;
        }

        public bool WriteIndented { get; private set; }

        public JsonSerializerOptions WithWriteIndented(bool value)
        {
            if (this.WriteIndented == value)
            {
                return this;
            }

            var result = new JsonSerializerOptions(this)
            {
                WriteIndented = value,
            };
            return result;
        }
    }
}
