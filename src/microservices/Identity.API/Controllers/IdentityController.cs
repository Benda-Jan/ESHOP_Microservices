using Microsoft.AspNetCore.Mvc;
using Identity.Entities.Dtos;
using Identity.Infrastructure;

namespace Identity.API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class IdentityController(IUserRepository userRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;

    [HttpPost]
    [Route("login")]
    public Task<IActionResult> Login(LoginInputDto loginInputDto)
        => HandleAction(async () => await _userRepository.ValidateUser(loginInputDto));

    [HttpPost]
    [Route("register")]
    public Task<IActionResult> Register(RegisterInputDto registerInputDto)
        => HandleAction(async () => await _userRepository.InsertUser(registerInputDto));

    [HttpGet]
    [Route("validate")]
    public async Task<IActionResult> ValidateToken([FromBody] TokenInputDto input)
    {
        // var user = await _userRepository.FindByEmail(email);

        // if (user is null)
        //     return NotFound("User not found.");

        // var userId = _jwtBuilder.ValidateToken(token);

        // if (userId != user.Id)
        //     return BadRequest("Invalid token.");

        // return Ok(userId);
        return Ok();
    }

    private async Task<IActionResult> HandleAction(Func<Task<object>> func)
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

