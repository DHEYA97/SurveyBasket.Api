using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Contract.Auth.Register;
using SurveyBasket.Api.Contract.ConfirmEmail;
using SurveyBasket.Api.Contract.ReSendConfirmEmail;
using SurveyBasket.Api.Contract.ResetPassword;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Api.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtProvider jwtProvider,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuthService> _logger = logger;

        private readonly int _refreshTokenExpiredDay = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var result = await _signInManager.PasswordSignInAsync(user,password,false,false);
            if (result.Succeeded)
            {
                var (token, expiration) = _jwtProvider.GenerateToken(user);

                //RefreshToken
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpierdDate = DateTime.UtcNow.AddDays(_refreshTokenExpiredDay);
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiredOn = refreshTokenExpierdDate
                });
                await _userManager.UpdateAsync(user);
                var authResponse = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, token, expiration, refreshToken, refreshTokenExpierdDate);
                return Result.Success(authResponse);
            }
            return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
            
        }
        
        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (token is null || refreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
            var userRefeshToken = user.RefreshTokens.SingleOrDefault(t=>t.Token == refreshToken && t.IsActive);
            if(userRefeshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
            userRefeshToken.RevokedOn = DateTime.UtcNow;
            var (newToken, expiration) = _jwtProvider.GenerateToken(user);

            //RefreshToken
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpierdDate = DateTime.UtcNow.AddDays(_refreshTokenExpiredDay);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiredOn = refreshTokenExpierdDate
            });
            await _userManager.UpdateAsync(user);
            var authResponse = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, newToken, expiration, newRefreshToken, refreshTokenExpierdDate);
            return Result.Success(authResponse);
        }

        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (token is null || refreshToken is null)
                return Result.Failure(UserErrors.InvalidJwtToken);
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure(UserErrors.InvalidJwtToken);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(UserErrors.InvalidJwtToken);
            var userRefeshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);
            if (userRefeshToken is null)
                return Result.Failure(UserErrors.InvalidRefreshToken);
            userRefeshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var isEmailExist = await _userManager.Users.AnyAsync(x=>x.Email == request.Email,cancellationToken);
            if(isEmailExist)
                return Result.Failure(UserErrors.DuplicatedEmail);
            
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);
            if(result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
                Dictionary<string, string> templateModel = new Dictionary<string, string>
                                {
                                    {"{{name}}",user.FirstName},
                                    {"{{action_url}}",$"{origin}/auth/confirm-email?UserId={user.Id}&Code={code}"}
                                };
                await SendEmailAsync(user, code, "EmailConfirmation", "✅ Survey Basket: Email Confirmation", templateModel);
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if(await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(UserErrors.InvalidCode);

            if(user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);
            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch(FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }
            var result = await _userManager.ConfirmEmailAsync(user,code);
            if (result.Succeeded)
            {
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ResendConfirmEmailAsync(ReSendConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            if(await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();
            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            Dictionary<string, string> templateModel = new Dictionary<string, string>
                                {
                                    {"{{name}}",user.FirstName},
                                    {"{{action_url}}",$"{origin}/auth/confirm-email?UserId={user.Id}&Code={code}"}
                                };
            await SendEmailAsync(user,code, "EmailConfirmation","✅ Survey Basket: Email Confirmation", templateModel);
            return Result.Success();
        }
        public async Task<Result> ForgetPasswordAsync(ReSendConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();
            if (!user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("code {code}",code);
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            Dictionary<string, string> templateModel = new Dictionary<string, string>
                                {
                                    {"{{name}}",user.FirstName},
                                    {"{{action_url}}",$"{origin}/auth/reset-password?email={user.Email}&Code={code}"}
                                };
            await SendEmailAsync(user, code, "ForgetPassword", "✅ Survey Basket: Forget Password Confirmation", templateModel);
            return Result.Success();
        }
        public async Task<Result> ConfirmResetPasswordAsync(ResetPasswordRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);
            
            var code = request.Code;
            IdentityResult result;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                result = await _userManager.ResetPasswordAsync(user, code,request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
                return Result.Failure(UserErrors.InvalidCode);
            }
            if (result.Succeeded)
            {
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
        }
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private async Task SendEmailAsync(ApplicationUser user, string code,string templateName,string title,Dictionary<string,string> templateModel)
        {
            var emailBody = EmailBodyBuilder.GenerateEmailBody(templateName,
                templateModel
                );
            //
            BackgroundJob.Enqueue(()=>_emailSender.SendEmailAsync(user.Email!, title, emailBody));
            await Task.CompletedTask;
        }
    }
}
