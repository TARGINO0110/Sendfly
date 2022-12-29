using Sendfly.Models.Mail;
using Sendfly.Services.Trataments.Interfaces;
using System.Dynamic;
using System.Globalization;

namespace Sendfly.Services.Trataments
{
    public class TratamentMail : ITratamentMail
    {
        private readonly IConfiguration _configuration;

        public TratamentMail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Prepare email sending data
        public dynamic ConfigDataMail(PostMail postMail)
        {
            dynamic dataMail = new ExpandoObject();
            dataMail.Subject = postMail.Subject;
            dataMail.Emails = string.Join(",", postMail.Contact.Email, _configuration.GetSection("SettingMailCeo:Mail").Value);
            dataMail.Id = postMail.Contact.Id;
            dataMail.Name = postMail.Contact.NamePerson;
            dataMail.Email = postMail.Contact.Email;
            dataMail.Phone = postMail.Contact.Phone;
            dataMail.Msg = postMail.Contact.Msg;
            dataMail.DtSend = DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("pt-BR"));

            return dataMail;
        }
    }
}
