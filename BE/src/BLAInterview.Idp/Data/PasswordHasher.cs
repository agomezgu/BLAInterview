using System.Security.Cryptography;

namespace BLAInterview.Idp.Data;

public sealed class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return string.Join(
            '.',
            "PBKDF2-SHA256",
            Iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts is not ["PBKDF2-SHA256", var iterationText, var saltText, var hashText])
        {
            return false;
        }

        if (!int.TryParse(iterationText, out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(saltText);
        var expectedHash = Convert.FromBase64String(hashText);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
