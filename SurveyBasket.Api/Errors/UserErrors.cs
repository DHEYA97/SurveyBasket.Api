namespace SurveyBasket.Api.Errors
{
    public class UserErrors
    {
       
        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Invalid email/password",StatusCodes.Status400BadRequest);

        public static readonly Error InvalidRole =
           new("User.InvalidRole", "Invalid Role", StatusCodes.Status400BadRequest);

        public static readonly Error NotFound =
            new("User.NotFound", "User Not Faound", StatusCodes.Status404NotFound);

        public static readonly Error DisabledUser =
           new("User.Disabled", "Disabled User Conntact to admin", StatusCodes.Status400BadRequest);

        public static readonly Error LockedOut =
          new("User.LockedOut", "User LockedOut Now", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status400BadRequest);
       
        public static readonly Error DuplicatedEmail =
            new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidCode =
            new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedConfirmation =
            new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);
    }
}
