using Sendfly.Models.Mail;

namespace Sendfly.Services.Trataments.Interfaces
{
    public interface ITratamentMail
    {
        dynamic ConfigDataMail(PostMail postMail);
    }
}
