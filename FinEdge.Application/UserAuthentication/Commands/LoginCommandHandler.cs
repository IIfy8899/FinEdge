using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace FinEdge.Application.UserAuthentication.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginCommandResult>>;

public class LoginCommandValidator: AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("email_is_required")
            .EmailAddress()
            .WithMessage("invalid_email_address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("password_is_required");
    }
}

public class LoginCommandResult
{
    public string Token { get; set; } = string.Empty;
}

public class LoginCommandHandler(
    IAuthService authService) : IRequestHandler<LoginCommand, Result<LoginCommandResult>>
{
    public async Task<Result<LoginCommandResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var token = await authService.Authenticate(request.Email, request.Password);
        if (token == null)
        {
            return Result<LoginCommandResult>.Failure(["invalid_credentials"]);
        }
        return Result<LoginCommandResult>.Success(new LoginCommandResult
        {
            Token = token
        });
    }
}
