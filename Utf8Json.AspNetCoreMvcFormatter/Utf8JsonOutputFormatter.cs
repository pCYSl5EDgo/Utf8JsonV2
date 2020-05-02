// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !CSHARP_8_OR_NEWER
using System;
#endif
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;

namespace Utf8Json.AspNetCoreMvcFormatter
{
    public sealed class JsonOutputFormatter : IOutputFormatter
    {
        private const string ContentType = "application/json";

        private readonly JsonSerializerOptions options;

        public JsonOutputFormatter(JsonSerializerOptions options)
        {
            this.options = options
#if !CSHARP_8_OR_NEWER
                ?? throw new ArgumentNullException(nameof(options))
#endif
                ;
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context.ContentType.HasValue)
            {
                return context.ContentType.Value == ContentType;
            }

            context.ContentType = new StringSegment(ContentType);
            return true;
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            context.HttpContext.Response.ContentType = ContentType;

            if (context.ObjectType != typeof(object))
            {
#if ASP_NET_CORE_APP_FRAMEWORK
                var writer = context.HttpContext.Response.BodyWriter;
                JsonSerializer.SerializeTypeless(context.ObjectType, writer, context.Object, this.options);
                return writer.FlushAsync().AsTask();
#else
                return JsonSerializer.SerializeTypelessAsync(context.ObjectType, context.HttpContext.Response.Body, context.Object, this.options, context.HttpContext.RequestAborted);
#endif
            }
            if (context.Object != null)
            {
#if ASP_NET_CORE_APP_FRAMEWORK
                var writer = context.HttpContext.Response.BodyWriter;
                JsonSerializer.SerializeTypeless(context.Object.GetType(), writer, context.Object, this.options);
                return writer.FlushAsync().AsTask();
#else
                return JsonSerializer.SerializeTypelessAsync(context.Object.GetType(), context.HttpContext.Response.Body, context.Object, this.options, context.HttpContext.RequestAborted);
#endif
            }

#if ASP_NET_CORE_APP_FRAMEWORK
            {
                var writer = context.HttpContext.Response.BodyWriter;
                var span = writer.GetSpan(4);
                span[0] = (byte)'n';
                span[1] = (byte)'u';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                writer.Advance(4);
                return writer.FlushAsync().AsTask();
            }
#else
            var responseBody = context.HttpContext.Response.Body;
            responseBody.WriteByte((byte)'n');
            responseBody.WriteByte((byte)'u');
            responseBody.WriteByte((byte)'l');
            responseBody.WriteByte((byte)'l');
            return Task.CompletedTask;
#endif
        }
    }
}
