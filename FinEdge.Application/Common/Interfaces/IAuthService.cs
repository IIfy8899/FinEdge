namespace FinEdge.Application.Common.Interfaces;

public interface IAuthService
{
    Task<string?> Authenticate(string email, string password);
}
