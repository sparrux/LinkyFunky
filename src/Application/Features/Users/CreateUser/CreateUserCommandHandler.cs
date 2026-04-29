using FluentResults;
using LinkyFunky.Application.Interfaces.Repositories;
using LinkyFunky.Domain.Entities;
using MediatR;

namespace LinkyFunky.Application.Features.Users.CreateUser;

/// <summary>
/// Handles user creation requests.
/// </summary>
public sealed class CreateUserCommandHandler(IUsersRepository usersRepository) : IRequestHandler<CreateUserCommand, Result<User>>
{
    /// <summary>
    /// Creates and persists a new <see cref="User"/>.
    /// </summary>
    /// <param name="request">The create user command.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>A result containing the created <see cref="User"/>.</returns>
    public async Task<Result<User>> Handle(CreateUserCommand request, CancellationToken ctk)
    {
        var user = User.Create();

        await usersRepository.AddAsync(user, ctk);
        await usersRepository.UnitOfWork.SaveChangesAsync(ctk);

        return Result.Ok(user);
    }
}
