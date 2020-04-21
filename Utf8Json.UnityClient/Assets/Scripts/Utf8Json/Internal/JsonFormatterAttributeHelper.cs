// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Utf8Json.Internal
{
    public static class JsonFormatterAttributeHelper
    {
        public static
#if CSHARP_8_OR_NEWER
            IJsonFormatter?
#else
            IJsonFormatter
#endif
            FromJsonFormatterAttribute(
#if CSHARP_8_OR_NEWER
                JsonFormatterAttribute?
#else
                JsonFormatterAttribute
#endif
                attribute)
        {
            if (attribute == null)
            {
                return default;
            }

            var formatterType = attribute.FormatterType;
            var arguments = attribute.Arguments;
            if (arguments == null || arguments.Length == 0)
            {
                var fieldOfInstance = formatterType.GetField("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (fieldOfInstance != null && fieldOfInstance.FieldType == formatterType)
                {
                    return fieldOfInstance.GetValue(null) as IJsonFormatter;
                }

                var fieldOfDefault = formatterType.GetField("Default", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (fieldOfDefault != null && fieldOfDefault.FieldType == formatterType)
                {
                    return fieldOfDefault.GetValue(null) as IJsonFormatter;
                }
            }

            var formatterObject = Activator.CreateInstance(formatterType, arguments);
            var formatter = formatterObject as IJsonFormatter;
            return formatter;
        }
    }
}