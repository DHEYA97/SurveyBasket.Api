﻿using SurveyBasket.Api.Contract.Auth.User;

namespace SurveyBasket.Api.Services
{
    public interface IUserService
    {
        Task<Result<UserResponse>> GetUserAsync(string userId,CancellationToken cancellationToken = default);
        Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    }
}
