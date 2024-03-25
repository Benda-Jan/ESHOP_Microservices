using System;
namespace Identity.Entities.Dtos;

public record LoginInputDto
{
	public required string Email { get; set; } = String.Empty;
	public required string Password { get; set; } = String.Empty;
}

