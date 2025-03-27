using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Common.Models;
using FinEdge.Application.UserWallet.Commands;
using FinEdge.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = FinEdge.Application.Exceptions.ValidationException;

namespace FinEdge.Application.UserAuthentication.Commands;

public record RegisterUserCommand(string Name, string Email, string Password) : IRequest<Result<RegisterUserCommandResult>>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("name_is_required")
            .MaximumLength(100)
            .WithMessage("name_must_not_exceed_100_characters")
            .MustAsync(BeUniqueName)
            .WithMessage("name_already_taken");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("email_is_required")
            .EmailAddress()
            .WithMessage("invalid_email_address")
            .MustAsync(BeUniqueEmail)
            .WithMessage("email_already_registered");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("password_is_required")
            .MinimumLength(8)
            .WithMessage("8_characters_minimum_for_password")
            .MaximumLength(50)
            .WithMessage("50_characters_max_for_password");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _userRepository.AnyAsync(u => u.Name == name);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.AnyAsync(u => u.Email == email);
    }
}

public class RegisterUserCommandResult
{
    public int UserId { get; set; }
    public int WalletId { get; set; }
}

public class RegisterUserCommandHandler(
    ILogger<RegisterUserCommandHandler> logger,
    IUserRepository userRepository,
    ISender mediator) : IRequestHandler<RegisterUserCommand, Result<RegisterUserCommandResult>>
{
    public async Task<Result<RegisterUserCommandResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        var result = await userRepository.AddAsync(user);
        if (result == 0)
        {
            return Result<RegisterUserCommandResult>.Failure(["failed_to_register_user"]);
        }

        var response = await mediator.Send(new CreateWalletCommand(user.Id), cancellationToken);
        if (!response.Succeeded)
        {
            logger.LogError("Failed to create wallet for user {UserId}", user.Id);
            return Result<RegisterUserCommandResult>.Failure(response.Errors);
        }

        var userInfo = new RegisterUserCommandResult
        {
            UserId = user.Id,
            WalletId = response.Data!.WalletId
        };

        return Result<RegisterUserCommandResult>.Success(userInfo);
    }
}
