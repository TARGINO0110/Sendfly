using SendGrid;

namespace Sendfly.Services.MailServices.Interfaces
{
    public interface IMailSenderService
    {
        Task<IEnumerable<Response>> SendMailAsync(dynamic dataMail);
    }
}
