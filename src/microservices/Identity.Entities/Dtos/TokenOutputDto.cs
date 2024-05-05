
namespace Identity.Entities.Dtos;

public record TokenOutputDto
{
    public required string Token { get; set; }
    public required string RefreshToken {get; set;}
}