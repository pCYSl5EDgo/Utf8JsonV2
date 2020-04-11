// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerOptions"/> class
        /// with members initialized from an existing instance.
        /// </summary>
        /// <param name="copyFrom">The options to copy from.</param>
        private JsonSerializerOptions(JsonSerializerOptions copyFrom)
        {
            this.Resolver = copyFrom.Resolver;
            this.OmitAssemblyVersion = copyFrom.OmitAssemblyVersion;
            this.AllowAssemblyVersionMismatch = copyFrom.AllowAssemblyVersionMismatch;
        }

        /// <summary>
        /// Gets the resolver to use for complex types.
        /// </summary>
        /// <value>An instance of <see cref="IFormatterResolver"/>. Never <c>null</c>.</value>
        /// <exception cref="ArgumentNullException">Thrown if an attempt is made to set this property to <c>null</c>.</exception>
        public IFormatterResolver Resolver { get; private set; }

        /// <summary>
        /// Gets a value indicating whether serialization should omit assembly version, culture and public key token metadata when using the typeless formatter.
        /// </summary>
        /// <value>The default value is <c>false</c>.</value>
        public bool OmitAssemblyVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether deserialization may instantiate types from an assembly with a different version if a matching version cannot be found.
        /// </summary>
        /// <value>The default value is <c>false</c>.</value>
        public bool AllowAssemblyVersionMismatch { get; private set; }

        internal static readonly Regex AssemblyNameVersionSelectorRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=[\w-]+, PublicKeyToken=(?:null|[a-f0-9]{16})$", RegexOptions.Compiled);

        /// <summary>
        /// Gets a type given a string representation of the type.
        /// </summary>
        /// <param name="typeName">The name of the type to load. This is typically the <see cref="Type.AssemblyQualifiedName"/> but may use the assembly's simple name.</param>
        /// <returns>The loaded type or <c>null</c> if no matching type could be found.</returns>
#if CSHARP_8_OR_NEWER
        public Type? LoadType(string typeName)
#else
        public Type LoadType(string typeName)
#endif
        {
            var result = Type.GetType(typeName, false);
            if (result != null || !this.AllowAssemblyVersionMismatch)
            {
                goto RETURN;
            }

            var shortenedName = AssemblyNameVersionSelectorRegex.Replace(typeName, string.Empty);
            if (shortenedName != typeName)
            {
                result = Type.GetType(shortenedName, false);
            }

        RETURN:
            return result;
        }

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

        /// <summary>
        /// Gets a copy of these options with the <see cref="Resolver"/> property set to a new value.
        /// </summary>
        /// <param name="resolver">The new value for the <see cref="Resolver"/>.</param>
        /// <returns>The new instance; or the original if the value is unchanged.</returns>
        public JsonSerializerOptions WithResolver(IFormatterResolver resolver)
        {
            if (this.Resolver == resolver)
            {
                return this;
            }

            var result = this.Clone();
            result.Resolver = resolver;
            return result;
        }

        /// <summary>
        /// Gets a copy of these options with the <see cref="OmitAssemblyVersion"/> property set to a new value.
        /// </summary>
        /// <param name="omitAssemblyVersion">The new value for the <see cref="OmitAssemblyVersion"/> property.</param>
        /// <returns>The new instance; or the original if the value is unchanged.</returns>
        public JsonSerializerOptions WithOmitAssemblyVersion(bool omitAssemblyVersion)
        {
            if (this.OmitAssemblyVersion == omitAssemblyVersion)
            {
                return this;
            }

            var result = this.Clone();
            result.OmitAssemblyVersion = omitAssemblyVersion;
            return result;
        }

        /// <summary>
        /// Gets a copy of these options with the <see cref="AllowAssemblyVersionMismatch"/> property set to a new value.
        /// </summary>
        /// <param name="allowAssemblyVersionMismatch">The new value for the <see cref="AllowAssemblyVersionMismatch"/> property.</param>
        /// <returns>The new instance; or the original if the value is unchanged.</returns>
        public JsonSerializerOptions WithAllowAssemblyVersionMismatch(bool allowAssemblyVersionMismatch)
        {
            if (this.AllowAssemblyVersionMismatch == allowAssemblyVersionMismatch)
            {
                return this;
            }

            var result = this.Clone();
            result.AllowAssemblyVersionMismatch = allowAssemblyVersionMismatch;
            return result;
        }

        /// <summary>
        /// Creates a clone of this instance with the same properties set.
        /// </summary>
        /// <returns>The cloned instance. Guaranteed to be a new instance.</returns>
        /// <exception cref="NotSupportedException">Thrown if this instance is a derived type that doesn't override this method.</exception>
        private JsonSerializerOptions Clone()
        {
            var clone = new JsonSerializerOptions(this);
            return clone;
        }
    }
}