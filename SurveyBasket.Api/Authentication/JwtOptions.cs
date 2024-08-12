using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Api.Authentication
{
    public class JwtOptions
    {
        public static string SectionName = "JWT";
        [Required]
        public string Key { get; init; } = string.Empty;
        [Required]
        public string Issuer { get; init; } = string.Empty;
        [Required]
        public string Audience { get; init; } = string.Empty;
        [Required]
        [Range(1,int.MaxValue)]
        public int Expirition { get; init; }
        public override string ToString()
        {
            return $"Key : {this.Key} - Issuer : {this.Issuer} - Audience : {this.Audience} - Expirition : {this.Expirition}";
        }
    }
}
