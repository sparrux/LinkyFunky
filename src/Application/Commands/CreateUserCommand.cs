using FluentResults;
using LinkyFunky.Domain.Entities;
using MediatR;

namespace LinkyFunky.Application.Commands;

/// <summary>
/// Creates a new anonymous user.
/// </summary>
public sealed record CreateUserCommand : IRequest<Result<User>>;
