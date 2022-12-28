using Sendfly.Models.Auth;

namespace Sendfly.Services.ServiceAuth.Interfaces
{
    public interface IJWTService
    {
        string GenerateToken(User user);
    }
}
