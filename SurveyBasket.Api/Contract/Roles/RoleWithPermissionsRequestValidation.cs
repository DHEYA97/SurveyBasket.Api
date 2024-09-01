namespace SurveyBasket.Api.Contract.Roles
{
    public class RoleWithPermissionsRequestValidation : AbstractValidator<RoleWithPermissionsRequest>
    {
        public RoleWithPermissionsRequestValidation()
        {
            RuleFor(x => x.Name)
                   .NotEmpty();
            
            RuleFor(x => x.Permissions)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Permissions)
                .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("You Cannot duplicate Permission")
                .When(x => x is not null);
        }
    }
}
