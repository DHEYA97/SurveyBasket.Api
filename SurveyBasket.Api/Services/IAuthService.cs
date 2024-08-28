using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Contract.Auth.Register;
using SurveyBasket.Api.Contract.ConfirmEmail;
using SurveyBasket.Api.Contract.ReSendConfirmEmail;
using SurveyBasket.Api.Contract.ResetPassword;
namespace SurveyBasket.Api.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email,string password,CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<Result> ResendConfirmEmailAsync(ReSendConfirmEmailRequest request, CancellationToken cancellationToken = default);
        Task<Result> ForgetPasswordAsync(ReSendConfirmEmailRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmResetPasswordAsync(ResetPasswordRequest request);
    }
}
