using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Roles;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet("")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAllRoles([FromQuery] bool isIncludeDeleted ,CancellationToken cancellationToken)
        {
            var result = await _roleService.GetAllRolesAsync(isIncludeDeleted,cancellationToken);
            return Ok(result);
        }
        [HttpGet("{roleId}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetRoleById([FromRoute] string roleId,CancellationToken cancellationToken)
        {
            var result = await _roleService.GetRoleByIdAsync(roleId,cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> AddRoleById([FromBody] RoleWithPermissionsRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.AddRolesAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetRoleById),new { roleId = result.Value.Id },result.Value) : result.ToProblem();
        }
        [HttpPut("{roleId}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> UpdateRole([FromRoute]string roleId, [FromBody] RoleWithPermissionsRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateRolesAsync(roleId,request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{roleId}/toggle-status")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> ToggleRoleById([FromRoute] string roleId, CancellationToken cancellationToken)
        {
            var result = await _roleService.ToggleRolesAsync(roleId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
