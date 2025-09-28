using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Api.Settinges
{
    public class MailSetting
    {
        public static string SectionName = "MailSettings";
        [Required, EmailAddress]
        public string Mail { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Host { get; set; }
        [Required, Range(100, 900)]
        public int Port { get; set; }
    }
}
