using SurveyBasket.Api.Contract.Roles;

namespace SurveyBasket.Api.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RolesResponse>> GetAllRolesAsync(bool? isIncludeDeleted = false, CancellationToken cancellationToken = default);
        Task<Result<RoleWithPermissionsResponse>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);
        Task<Result<RoleWithPermissionsResponse>> AddRolesAsync(RoleWithPermissionsRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateRolesAsync(string roleId, RoleWithPermissionsRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleRolesAsync(string roleId, CancellationToken cancellationToken = default);
    }
}
