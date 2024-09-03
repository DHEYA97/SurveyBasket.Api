
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Persistence;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Api.Services
{
    public class NotificationService(
                    ApplicationDbContext context,
                    UserManager<ApplicationUser> userManager,
                    IHttpContextAccessor httpContextAccessor,
                    IEmailSender emailSender
        ) : INotificationService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task SendEmailInBackgroundJob(int? pollId)
        {
            IEnumerable<Poll> pollList = [];
            if(pollId.HasValue)
            {
                var poll = _context.Polls
                                   .SingleOrDefault(x=>x.Id == pollId && x.IsPublished && x.StartAt == DateOnly.FromDateTime(DateTime.UtcNow));
                pollList = [poll!];
            }
            else
            {
                pollList = await _context.Polls
                                         .Where(x=> x.IsPublished && x.StartAt == DateOnly.FromDateTime(DateTime.UtcNow))
                                         .ToListAsync();
            }
            var users = await _userManager.Users.ToListAsync();
            foreach( var poll in pollList ) 
            {
                foreach (var user in users)
                {
                    var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
                    var emailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification",
                        new Dictionary<string, string>
                        {
                            {"{{name}}",user.FirstName},
                            {"{{pollTill}}",poll.Title},
                            {"{{endDate}}",poll.EndAt.ToString()},
                            {"{{url}}",$"{origin}/poll/polls{poll.Id}/vote"}
                        });
                    await _emailSender.SendEmailAsync(user.Email!, $"📣 Survey Basket: New Poll - {poll.Title}", emailBody);
                }
            }
            
        }
    }
}
