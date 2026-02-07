using CoreFlow.Shared.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CoreFlow.Api.Extensions
{
    public static class HttpResultExtensions
    {
        public static IResult ToHttpResult(this Result result)
            => Results.Json(result, statusCode: (int)result.StatusCode);

        public static IResult ToHttpResult<T>(this Result<T> result)
            => Results.Json(result, statusCode: (int)result.StatusCode);
    }
}
