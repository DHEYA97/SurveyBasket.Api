namespace SurveyBasket.Api.Errors
{
    public static class PollErrors
    {
        public static readonly Error PollNotFound =
            new("Poll.NotFound", "No poll was found with the given ID",StatusCodes.Status404NotFound);
        public static readonly Error DuplicatePollTitle =
           new("Poll.DuplicateTitle", "Another poll with The Same Poll Title", StatusCodes.Status409Conflict);
    }
}
