namespace LinkyFunky.Application.Contracts.Responses;

public sealed class ErrorResponseList : List<ErrorResponse>;

public sealed record ErrorResponse(string Message);