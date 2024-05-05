
namespace Identity.Entities.Dtos;

public record TokenInputDto
{
    public required string Email { get; set; }
    public required string Token { get; set; }
}