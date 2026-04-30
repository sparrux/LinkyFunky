using FastEndpoints;
using FluentResults;
using LinkyFunky.Application.Common.Mappers;

namespace Web.Extensions;

internal static class HttpResponseExtensions
{
    public static Task SendResultResponseAsync<T>(
        this HttpResponse response, 
        Result<T> result,
        int successCode = 200,
        int errorCode = 400,
        CancellationToken ctk = default)
    {
        if (result.IsSuccess)
        {
            response.StatusCode = successCode;
            return response.WriteAsJsonAsync(result.Value, ctk);
        }

        response.StatusCode = errorCode;
        return response.WriteAsJsonAsync(result.ToErrorResponse(), ctk);
    }
    
    public static Task SendResultResponseAsync(
        this HttpResponse response, 
        Result result,
        int successCode = 200,
        int errorCode = 400,
        CancellationToken ctk = default)
    {
        if (result.IsSuccess)
        {
            response.StatusCode = successCode;
            return response.SendStatusCodeAsync(successCode, ctk);
        }

        response.StatusCode = errorCode;
        return response.WriteAsJsonAsync(result.ToErrorResponse(), ctk);
    }
}