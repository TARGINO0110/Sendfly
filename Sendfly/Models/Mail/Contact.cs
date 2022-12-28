namespace Sendfly.Models.Mail
{
    public class Contact
    {
        public string Id { get => Guid.NewGuid().ToString(); }
        public string NamePerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Msg { get; set; }
    }
}
