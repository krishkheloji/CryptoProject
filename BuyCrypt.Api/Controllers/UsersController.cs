using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Helper;
using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Get()
    {
        var userId = User.GetUserId();
        return Ok(await _service.GetOrCreateAsync(userId));
    }

    [HttpPut("me")]
    public async Task<IActionResult> Update(UpdateUserRequest request)
    {
        var userId = User.GetUserId();
        return Ok(await _service.UpdateAsync(userId, request));
    }

    [HttpDelete("me")]
    public async Task<IActionResult> Delete()
    {
        var userId = User.GetUserId();
        await _service.DeleteAsync(userId);
        return NoContent();
    }
}
