namespace SurveyBasket.Api.Errors
{
    public static class QuestionErrors
    {
        public static readonly Error QuestionNotFound =
            new("Question.NotFound", "No Question was found with the given ID", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateQuestionContent =
           new("Question.DuplicateContent", "Another Question with The Same Content", StatusCodes.Status409Conflict);
    }
}