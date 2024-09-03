namespace SurveyBasket.Api.Services
{
    public interface INotificationService
    {
        Task SendEmailInBackgroundJob(int? pollId);
    }
}
