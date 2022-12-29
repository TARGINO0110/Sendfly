using System.Text.Json.Serialization;

namespace Sendfly.Models.Mail
{
    public class PostMail
    {
        public string Subject { get; set; }
        public Contact Contact { get; set; }
    }
}
