using SurveyBasket.Api.Contract.Auth.Register;
using SurveyBasket.Api.Contract.Auth.User;
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

            config.NewConfig<(ApplicationUser user,IList<string> role), UserDetailsResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.role);

            config.NewConfig<AddUserRequest, ApplicationUser>()
                .Map(des=>des.UserName,src=>src.Email)
                .Map(des=>des.EmailConfirmed,src=>true);

            config.NewConfig<UpdateUserRequest, ApplicationUser>()
                .Map(des => des.UserName, src => src.Email)
                .Map(des => des.NormalizedUserName, src => src.Email.ToUpper());
        }
    }
}
