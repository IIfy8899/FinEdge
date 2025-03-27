using FinEdge.Application.Common.Interfaces;
using FinEdge.Application.Common.Models;
using FinEdge.Domain.Entities;
using MediatR;

namespace FinEdge.Application.UserWallet.Commands;

public record CreateWalletCommand(int UserId) : IRequest<Result<CreateWalletCommandResult>>;

public class CreateWalletCommandResult
{
    public int WalletId { get; set; }
}

public class CreateWalletCommandHandler(
    IWalletRepository walletRepository,
    IUserRepository userRepository) : IRequestHandler<CreateWalletCommand, Result<CreateWalletCommandResult>>
{
    public async Task<Result<CreateWalletCommandResult>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Result<CreateWalletCommandResult>.Failure(["user_not_found"]);

        var wallet = new Wallet
        {
            UserId = request.UserId,
            Balance = 0
        };

        var response = await walletRepository.AddAsync(wallet);
        if (response == 0)
            return Result<CreateWalletCommandResult>.Failure(["failed_to_create_wallet"]);

        return Result<CreateWalletCommandResult>.Success(new CreateWalletCommandResult
        {
            WalletId = wallet.Id
        });
    }
}
