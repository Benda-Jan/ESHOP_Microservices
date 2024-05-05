using System.Diagnostics.CodeAnalysis;

namespace Identity.Entities.DbSet;

public class User
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string Salt { get; set; } = String.Empty;
    public bool IsAdmin { get; set; }

    [SetsRequiredMembers]
    public User(string username, string email, string password, IEncryptor encryptor)
    {
        Id = Guid.NewGuid().ToString();
        Username = username;
        Email = email;
        Salt = encryptor.GetSalt();
        Password = encryptor.GetHash(password, Salt);
    }

    public User()
    {
    }

    public bool ValidatePassword(string password, IEncryptor encryptor) =>
        Password == encryptor.GetHash(password, Salt);
}

