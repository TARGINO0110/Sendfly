using Sendfly.Models.Auth;
using Sendfly.Services.ServiceAuth.Interfaces;

namespace Sendfly.Services.ServiceAuth
{
    public class CredentialRepository : ICredentialRepository
    {
        private readonly IJWTService _jWTService;

        public CredentialRepository(IJWTService jWTService)
        {
            _jWTService = jWTService;
        }

        public object PrepareCredentials(User user)
        {
            try
            {
                var result = Get(user);
                if (result is null)
                    return null;

                //Generate jwt token according to credentials
                var token = _jWTService.GenerateToken(user);
                var dataHoraValidado = DateTime.Now;
                var dataExpira = TimeSpan.FromHours(24);
                DateTime dt = dataHoraValidado + dataExpira;

                return new
                {
                    token,
                    dataHoraValidado = dataHoraValidado.ToString(),
                    dataHoraExpira = dt.ToString()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static User Get(User user)
        {
            return _ = (user.Username == "<your username>" && user.Password == "your password" && user.Role == "your permission") ? user : null;
        }
    }
}
