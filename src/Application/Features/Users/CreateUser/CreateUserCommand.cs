using FluentResults;
using LinkyFunky.Domain.Entities;
using MediatR;

namespace LinkyFunky.Application.Features.Users.CreateUser;

/// <summary>
/// Creates a new anonymous user.
/// </summary>
public sealed record CreateUserCommand : IRequest<Result<User>>;
