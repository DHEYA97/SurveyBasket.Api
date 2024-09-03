using SurveyBasket.Api.Contract.Auth.User;

namespace SurveyBasket.Api.Services
{
    public interface IUserService
    {
        Task<Result<UserResponse>> GetUserAsync(string userId,CancellationToken cancellationToken = default);
        Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<IEnumerable<UserDetailsResponse>> GetAllUserAsync(CancellationToken cancellationToken = default);
        Task<Result<UserDetailsResponse>> GetUserDetailsAsync(string Id, CancellationToken cancellationToken = default);
        Task<Result<UserDetailsResponse>> AddUserAsync(AddUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateUserAsync(string Id, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(string Id, CancellationToken cancellationToken = default);
        Task<Result> UnLockAsync(string Id, CancellationToken cancellationToken = default);
    }
}
