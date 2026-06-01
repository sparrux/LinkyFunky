using FluentResults;
using LinkyFunky.Domain.Entities;
using MediatR;

namespace LinkyFunky.Application.Features.Users.CreateUser;

/// <summary>
/// Creates a new user.
/// <param name="UserId">Specified user id.</param>
/// </summary>
public sealed record CreateUserCommand(Guid UserId = default) : IRequest<Result<User>>;
