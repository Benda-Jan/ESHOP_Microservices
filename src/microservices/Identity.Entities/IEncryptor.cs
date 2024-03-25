using System;
namespace Identity.Entities;

public interface IEncryptor
{
    string GetSalt();
    string GetHash(string value, string salt);
}

