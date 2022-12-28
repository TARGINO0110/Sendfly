using Microsoft.Extensions.Options;
using Sendfly.Models.Mail;
using Sendfly.Services.MailServices.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using static Sendfly.Utils.Templates.TemplatesMail;

namespace Sendfly.Services.MailServices
{
    public class MailSenderService : IMailSenderService
    {
        public SendMail Settings { get; set; }
        private readonly IConfiguration _configuration;

        public MailSenderService(IOptions<SendMail> settings, IConfiguration configuration)
        {
            Settings = settings.Value;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Response>> SendMailAsync(dynamic dataMail)
        {
            string messageFooter = new TemplateContact().ContactSenderMail();
            return await ExecuteMail(Settings.ApiKey, messageFooter, dataMail);
        }

        private async Task<IEnumerable<Response>> ExecuteMail(string apiKey, string messageFooter, dynamic dataMail)
        {
            try
            {
                //Validate credentials
                var client = new SendGridClient(apiKey);

                var msg = new SendGridMessage
                {
                    From = new EmailAddress(Settings.SenderMail, Settings.SenderName),
                    Subject = dataMail.Subject
                };

                //email array
                string[] mailsArray = dataMail.Emails.Split(',');

                List<EmailAddress> mails = new();
                List<Response> responses = new();

                foreach (var value in mailsArray)
                {
                    mails.Add(new EmailAddress(value));

                    //Conditions Customer and CEO template
                    msg.TemplateId = value == _configuration.GetSection("SettingMailCeo:Mail").Value ? "<CEO TEMPLATE>" : "<CLIENT TEMPLATE>";

                    msg.AddTo(new EmailAddress(value));

                    //SendGrid Template Body Values
                    dynamic dataMailTemplate = new
                    {
                        msg = dataMail.Msg,
                        name = dataMail.Name,
                        email = dataMail.Email,
                        phone = dataMail.Phone,
                        footer = messageFooter,
                        protocol = dataMail.Id,
                        dataSend = dataMail.DtSend,
                    };

                    msg.AddTo(new EmailAddress(value));
                    msg.SetTemplateData(dataMailTemplate);

                    //Disabled settings for SEND GRID
                    msg.SetClickTracking(false, false);
                    msg.SetOpenTracking(false);
                    msg.SetGoogleAnalytics(false);
                    msg.SetSubscriptionTracking(false);
                    //Returns send grid server information
                    responses.Add(await client.SendEmailAsync(msg));
                }

                return responses.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
