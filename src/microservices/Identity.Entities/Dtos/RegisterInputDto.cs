using System;
namespace Identity.Entities.Dtos;

public record RegisterInputDto
{
    public required string Email { get; set; } = String.Empty;
    public required string Password { get; set; } = String.Empty;
    public required string PasswordConfirmation { get; set; } = String.Empty;
}

