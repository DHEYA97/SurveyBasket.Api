using SurveyBasket.Api.Contract.Roles;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Persistence;
using System.Data;

namespace SurveyBasket.Api.Services
{
    public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext context) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<RolesResponse>> GetAllRolesAsync(bool? isIncludeDeleted = false,CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles
                                         .Where(x => !x.IsDefault && (!x.IsDeleted || (isIncludeDeleted.HasValue && isIncludeDeleted.Value)))
                                         .ProjectToType<RolesResponse>()
                                         .ToListAsync(cancellationToken);
            return roles;
        }
        public async Task<Result<RoleWithPermissionsResponse>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            var role = await _roleManager.Roles
                                         .Where(x => x.Id == roleId)
                                         .SingleOrDefaultAsync(cancellationToken);
            if (role is null) {
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.RoleNotFound);
            }
            var permission = await _roleManager.GetClaimsAsync(role);
            var roleWithPermissions = new RoleWithPermissionsResponse(
                role.Id,
                role.Name!,
                role.IsDeleted,
                permission.Select(x => x.Value).ToList());
            return Result.Success(roleWithPermissions);
        }
        public async Task<Result<RoleWithPermissionsResponse>> AddRolesAsync(RoleWithPermissionsRequest request, CancellationToken cancellationToken = default)
        {
            if(await _roleManager.RoleExistsAsync(request.Name))
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.DuplicateRole);
            
            if(request.Permissions.Except(Permissions.GetAllPermissions()).Any())
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.InvalidPermissions);
            var applicationRole = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            var result = await _roleManager.CreateAsync(applicationRole);
            if(result.Succeeded)
            {
                var permission = request.Permissions
                                        .Select(x => new IdentityRoleClaim<string>
                                        {
                                            ClaimType = Permissions.Type,
                                            RoleId = applicationRole.Id,
                                            ClaimValue = x,
                                        });

                await _context.AddRangeAsync(permission,cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var roleWithPermissions = new RoleWithPermissionsResponse(
                applicationRole.Id,
                applicationRole.Name!,
                applicationRole.IsDeleted,
                request.Permissions);
                return Result.Success(roleWithPermissions);
            }
            var error = result.Errors.First();
            return Result.Failure<RoleWithPermissionsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateRolesAsync(string roleId ,RoleWithPermissionsRequest request, CancellationToken cancellationToken = default)
        {
            var isNameExist = await _roleManager.Roles
                                                .Where(x => x.Name == request.Name && x.Id != roleId)
                                                .AnyAsync(cancellationToken);
            if (isNameExist)
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.DuplicateRole);
            
            if (await _roleManager.FindByIdAsync(roleId) is not { } role)
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.RoleNotFound);

            if (request.Permissions.Except(Permissions.GetAllPermissions()).Any())
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.InvalidPermissions);

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                var currentPermission = await _context.RoleClaims
                                                      .Where(x => x.RoleId == role.Id && x.ClaimType == Permissions.Type)
                                                      .Select(x => x.ClaimValue)
                                                      .ToListAsync(cancellationToken);
                                        
                var newPermission = request.Permissions.Except(currentPermission)
                                           .Select(x => new IdentityRoleClaim<string>
                                           {
                                               ClaimType = Permissions.Type,
                                               RoleId = role.Id,
                                               ClaimValue = x,
                                           });
                
                var removedPermission = currentPermission.Except(request.Permissions);
                await _context.RoleClaims
                              .Where(x=>x.RoleId == role.Id && removedPermission.Contains(x.ClaimValue))
                              .ExecuteDeleteAsync(cancellationToken);

                await _context.AddRangeAsync(newPermission, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ToggleRolesAsync(string roleId, CancellationToken cancellationToken = default)
        {
            
            if (await _roleManager.FindByIdAsync(roleId) is not { } role)
                return Result.Failure<RoleWithPermissionsResponse>(RoleErrors.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
                return Result.Success();
            
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    }
}
