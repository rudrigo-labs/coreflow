using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var cid = context.Request.Headers.ContainsKey(HeaderName)
                ? context.Request.Headers[HeaderName].ToString()
                : string.Empty;

            if (string.IsNullOrWhiteSpace(cid))
                cid = Guid.NewGuid().ToString("N");

            context.Response.Headers[HeaderName] = cid;

            using (LogContext.PushProperty("CorrelationId", cid))
            {
                await _next(context);
            }
        }
    }
}

