// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Utf8Json.AspNetCoreMvcFormatter
{
    public sealed class JsonInputFormatter : IInputFormatter
    {
        private const string ContentType = "application/json";

        private readonly JsonSerializerOptions options;

        public JsonInputFormatter(JsonSerializerOptions options)
        {
            this.options = options;
        }

        public bool CanRead(InputFormatterContext context)
        {
            return context.HttpContext.Request.ContentType == ContentType;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var result = await JsonSerializer.DeserializeTypelessAsync(context.ModelType, request.Body, this.options, context.HttpContext.RequestAborted).ConfigureAwait(false);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }
    }
}
