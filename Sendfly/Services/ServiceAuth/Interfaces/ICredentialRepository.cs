using Sendfly.Models.Auth;

namespace Sendfly.Services.ServiceAuth.Interfaces
{
    public interface ICredentialRepository
    {
        object PrepareCredentials(User user);
    }
}
