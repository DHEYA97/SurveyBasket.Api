using SurveyBasket.Api.Contract.Auth.User;

namespace SurveyBasket.Api.Services
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Result<UserResponse>> GetUserAsync(string userId,CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                                         .Where(x=>x.Id == userId)
                                         .ProjectToType<UserResponse>()
                                         .SingleAsync(cancellationToken);
            return Result.Success(user);
        }

        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user = request.Adapt(user);
            await _userManager.UpdateAsync(user!);
            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword,request.NewPassword);
            if(result.Succeeded)
                return Result.Success();
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

    }
}
