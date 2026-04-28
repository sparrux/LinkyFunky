using FluentResults;
using MediatR;

namespace LinkyFunky.Application.Commands;

/// <summary>
/// Requests a long URL by the provided short code.
/// </summary>
/// <param name="ShortCode">The short code of the shortcut.</param>
public sealed record GetShortcutLongUrlCommand(string ShortCode) : IRequest<Result<string>>;
