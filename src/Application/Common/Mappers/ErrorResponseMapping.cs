using FluentResults;
using LinkyFunky.Application.Contracts.Responses;

namespace LinkyFunky.Application.Common.Mappers;

public static class ErrorResponseMapping
{
    public static ErrorResponseList ToErrorResponse(this Result result)
    {
        return result.Errors.ToErrorResponse();
    }
    
    public static ErrorResponseList ToErrorResponse<T>(this Result<T> result)
    {
        return result.Errors.ToErrorResponse();
    }

    static ErrorResponseList ToErrorResponse(this List<IError> errors)
    {
        var list = new ErrorResponseList();
        list.AddRange(errors.Select(x => new ErrorResponse(x.Message)));

        return list;
    }
}