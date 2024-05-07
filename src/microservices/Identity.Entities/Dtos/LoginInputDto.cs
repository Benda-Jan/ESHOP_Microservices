using System;
using System.Text.Json.Serialization;
namespace Identity.Entities.Dtos;

public record LoginInputDto
{
	public required string Email { get; set; }
	public required string Password { get; set; }
}

