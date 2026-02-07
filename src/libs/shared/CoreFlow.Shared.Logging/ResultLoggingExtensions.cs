using CoreFlow.Shared.Logging;
using CoreFlow.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreFlow.Shared.Logging
{
    public static class ResultLoggingExtensions
    {
        public static Result WithLog<T>(this Result result, ILoggerService<T> logger, string? area = null, SeverityLevel level = SeverityLevel.Error)
        {
            logger.TrackResultTrace(result, area ?? typeof(T).Name, level);
            return result;
        }

        public static Result<TData> WithLog<T, TData>(this Result<TData> result, ILoggerService<T> logger, string? area = null, SeverityLevel level = SeverityLevel.Error)
        {
            logger.TrackResultTrace(result, area ?? typeof(T).Name, level);
            return result;
        }
    }
}

