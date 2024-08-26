using SurveyBasket.Api.Contract.Auth.Register;
using SurveyBasket.Api.Contract.Question;

namespace SurveyBasket.Api.Mapping
{
    public class MappingConfigration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //Add answer to question With out Mapster
            //config.NewConfig<QuestionRequest, Question>()
            //      .Ignore(nameof(QuestionRequest.Answers));

            config.NewConfig<QuestionRequest, Question>()
                  .Map(des => des.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

            config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);
        }
    }
}
