namespace SurveyBasket.Api.Errors
{
    public static class VoteErrors
    {
        public static readonly Error VoteNotFound =
            new("Vote.NotFound", "No Vote was found In poll", StatusCodes.Status404NotFound);
        public static readonly Error InvalidQuestions =
        new("Vote.InvalidQuestions", "Invalid questions", StatusCodes.Status400BadRequest);

        public static readonly Error DuplicatedVote =
            new("Vote.DuplicatedVote", "This user already voted before for this poll", StatusCodes.Status409Conflict);
    }
}
