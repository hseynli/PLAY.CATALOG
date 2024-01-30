using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Service.DTOs;
using Play.Identity.Service.Entities;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Play.Identity.Service.Controllers
{
    [Route("users")]
    [ApiController]
    [Authorize(Policy = LocalApi.PolicyName, Roles = Roles.Admin)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            IEnumerable<UserDto> user = _userManager.Users.ToList().Select(p => p.AsDto());

            await Task.CompletedTask;

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(Guid id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());

            return user is null ? (ActionResult<UserDto>)NotFound() : (ActionResult<UserDto>)Ok(user.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UserDto userDto)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
            {
                return NotFound();
            }

            user.Email = userDto.Email;
            user.UserName = userDto.Username;
            user.Gil = userDto.Gil;

            IdentityResult result = await _userManager.UpdateAsync(user);

            return result.Succeeded ? NoContent() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
            {
                return NotFound();
            }

            IdentityResult result = await _userManager.DeleteAsync(user);

            return result.Succeeded ? NoContent() : BadRequest();
        }
    }
}
