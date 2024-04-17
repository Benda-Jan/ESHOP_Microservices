using Microsoft.AspNetCore.Mvc;
using Identity.Entities.Dtos;
using Identity.Infrastructure;
using Identity.Entities;
using Identity.Entities.DbSet;
using JwtLibrary;

namespace Identity.API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IEncryptor _encryptor;
    private readonly ILogger<IdentityController> _logger;
    private readonly IJwtBuilder _jwtBuilder;

    public IdentityController(IUserRepository userRepository, IEncryptor encryptor, ILogger<IdentityController> logger, IJwtBuilder jwtBuilder)
    {
        _userRepository = userRepository;
        _encryptor = encryptor;
        _logger = logger;
        _jwtBuilder = jwtBuilder;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody]LoginInputDto loginInputDto)
    {
        var user = await _userRepository.GetUser(loginInputDto.Email);

        if (user == null)
            return NotFound($"User with email: {loginInputDto.Email} does not exist");

        if (!user.ValidatePassword(loginInputDto.Password, _encryptor))
            return BadRequest("Incorrect password");

        return Ok(_jwtBuilder.GetToken(user.Id!));
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody]RegisterInputDto registerInputDto)
    {
        if (registerInputDto.Password != registerInputDto.PasswordConfirmation)
            return BadRequest($"Passwords are not the same");

        var newUser = new User(registerInputDto.Email, registerInputDto.Password, _encryptor);

        try
        {
            await _userRepository.InsertUser(newUser);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpGet("validate")]
    public async Task<ActionResult> Validate([FromQuery(Name = "email")] string email,
                                 [FromQuery(Name = "token")] string token)
    {
        var u = await _userRepository.GetUser(email);

        if (u is null)
            return NotFound("User not found.");


        var userId = _jwtBuilder.ValidateToken(token);

        if (userId != u.Id)
            return BadRequest("Invalid token.");

        return Ok(userId);
    }
}

