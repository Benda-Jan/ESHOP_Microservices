using System.Diagnostics.CodeAnalysis;

namespace Identity.Entities.DbSet;

public class User
{
    public string? Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string Salt { get; set; } = String.Empty;
    public bool IsAdmin { get; set; }

    [SetsRequiredMembers]
    public User(string email, string password, IEncryptor encryptor)
    {
        Id = Guid.NewGuid().ToString();
        Username = email;
        Email = email;
        SetPassword(password, encryptor);
    }

    public User()
    {
    }

    public void SetPassword(string password, IEncryptor encryptor)
    {
        Salt = encryptor.GetSalt();
        Password = encryptor.GetHash(password, Salt);
    }

    public bool ValidatePassword(string password, IEncryptor encryptor) =>
        Password == encryptor.GetHash(password, Salt);
}

