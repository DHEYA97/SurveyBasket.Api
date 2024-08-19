using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Abstractions
{
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException();
            var problem = Results.Problem(statusCode: result.Error.status);
            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
                       {
                           { "errors",new[]
                                {
                                   new 
                                   {
                                        result.Error.Code,
                                        result.Error.Description
                                   }
                                } 
                           }
                       };
            return new ObjectResult(problemDetails);
        }
    }
}
