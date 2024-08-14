﻿namespace SurveyBasket.Api.Errors
{
    public class UserErrors
    {
       
        public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password",StatusCodes.Status400BadRequest);

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status400BadRequest);
    }
}
