using SurveyBasket.Api.Contract.Auth.User;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager, 
        ApplicationDbContext context,
        IRoleService roleService
        ) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        private readonly IRoleService _roleService = roleService;

        public async Task<IEnumerable<UserDetailsResponse>> GetAllUserAsync(CancellationToken cancellationToken = default) =>
                  await (from u in _context.Users
                         join ur in _context.UserRoles
                         on u.Id equals ur.UserId
                         join r in _context.Roles
                         on ur.RoleId equals r.Id
                         into role
                         where role.Any(x=>x.Name != DefaultRoles.Member)
                         select new
                         {
                             u.Id,
                             u.UserName,
                             u.FirstName,
                             u.LastName,
                             u.Email,
                             u.IsDisabled,
                             Roles = role.Select(x => x.Name).ToList(),
                         })
                        .GroupBy(u => new { u.Id, u.UserName, u.FirstName, u.LastName, u.Email, u.IsDisabled})
                        .Select(g => new UserDetailsResponse
                                            (
                                                g.Key.Id,
                                                g.Key.FirstName,
                                                g.Key.LastName,
                                                g.Key.Email!,
                                                g.Key.IsDisabled,
                                                g.SelectMany(x=>x.Roles).ToList()
                                            ))
                        .ToListAsync(cancellationToken);
        public async Task<Result<UserDetailsResponse>> GetUserDetailsAsync(string Id,CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByIdAsync(Id) is not { } user)
               return Result.Failure<UserDetailsResponse>(UserErrors.NotFound);

            var role = await _userManager.GetRolesAsync(user);
            var userResponse = (user, role).Adapt<UserDetailsResponse>();
            return Result.Success(userResponse);
        }
        public async Task<Result<UserResponse>> GetUserAsync(string userId,CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                                         .Where(x=>x.Id == userId)
                                         .ProjectToType<UserResponse>()
                                         .SingleAsync(cancellationToken);
            return Result.Success(user);
        }
        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            //var user = await _userManager.FindByIdAsync(userId);
            //user = request.Adapt(user);
            //await _userManager.UpdateAsync(user!);
            
            var user = await _userManager.Users
                                         .Where(x=>x.Id == userId)
                                         .ExecuteUpdateAsync(setters=>
                                                setters
                                                    .SetProperty(x=>x.FirstName , request.FirstName)
                                                    .SetProperty(x => x.LastName, request.LastName)

                                         );
            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword,request.NewPassword);
            if(result.Succeeded)
                return Result.Success();
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result<UserDetailsResponse>> AddUserAsync(AddUserRequest request,CancellationToken cancellationToken = default)
        {
            var IsEmailExist = await _userManager.Users.AnyAsync(x=>x.Email == request.Email,cancellationToken);
            if (IsEmailExist)
                return Result.Failure<UserDetailsResponse>(UserErrors.DuplicatedEmail);

            var currentRole = await _roleService.GetAllRolesAsync(cancellationToken: cancellationToken);
            var currentRoleList = currentRole.Select(x=>x.Name).ToList();

            if(request.Roles.Except(currentRoleList).Any())
                return Result.Failure<UserDetailsResponse>(UserErrors.InvalidRole);

            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user,request.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user,request.Roles);
                var response = (user, request.Roles).Adapt<UserDetailsResponse>();
                return Result.Success<UserDetailsResponse>(response);
            }
            var error = result.Errors.First();
            return Result.Failure<UserDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateUserAsync(string Id,UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var IsEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != Id, cancellationToken);
            if (IsEmailExist)
                return Result.Failure(UserErrors.DuplicatedEmail);

            var currentRole = await _roleService.GetAllRolesAsync(cancellationToken: cancellationToken);
            var currentRoleList = currentRole.Select(x => x.Name).ToList();

            if (request.Roles.Except(currentRoleList).Any())
                return Result.Failure(UserErrors.InvalidRole);

            if (await _userManager.FindByIdAsync(Id) is not { } user)
                return Result.Failure(UserErrors.NotFound);

            user = request.Adapt(user);
            
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _context.UserRoles
                              .Where(x => x.UserId == Id)
                              .ExecuteDeleteAsync(cancellationToken);
                await _userManager.AddToRolesAsync(user,request.Roles);
              
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ToggleStatusAsync(string Id,CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByIdAsync(Id) is not { } user)
                return Result.Failure(UserErrors.NotFound);

            user.IsDisabled = !user.IsDisabled;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Result.Success();
            
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UnLockAsync(string Id, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByIdAsync(Id) is not { } user)
                return Result.Failure(UserErrors.NotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user,null);
            
            if (result.Succeeded)
                return Result.Success();
            
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    }
}
