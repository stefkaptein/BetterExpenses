using System.Security.Cryptography;

namespace BetterExpenses.API.Helpers;

public static class PinHelper
{
    private const int SaltLength = 16;
    private const int HashLength = 32;
    
    public static string GetHashedPinWithSalt(string pin)
    {
        var salt = GenerateSalt();
        var hash = Hash(pin, salt);
        var combined = CombineSaltAndHash(salt, hash);
        return Convert.ToBase64String(combined);
    }

    public static bool PinMatchesHash(string base64Hash, string pin)
    {
        var combinedBytes = Convert.FromBase64String(base64Hash);
        var split = SplitSaltAndHash(combinedBytes);
        var salt = split[0];
        var hash = split[1];

        var pinHash = Hash(pin, salt);

        return hash.SequenceEqual(pinHash);
    }

    private static byte[] CombineSaltAndHash(byte[] salt, byte[] hash)
    {
        var combined = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);
        return combined;
    }

    /// <summary>
    /// Returns a two dimensional array where the first array contains the salt and the second array contains the hash.
    /// </summary>
    /// <param name="combined"></param>
    /// <returns></returns>
    private static byte[][] SplitSaltAndHash(byte[] combined)
    {
        var result = new byte[2][];
        result[0] = new byte[SaltLength];
        result[1] = new byte[HashLength];
        Buffer.BlockCopy(combined, 0, result[0], 0, SaltLength);
        Buffer.BlockCopy(combined, SaltLength, result[1], 0, HashLength);
        return result;
    }
    
    private static byte[] GenerateSalt()
    {
        // Generate a random 16-byte salt
        return RandomNumberGenerator.GetBytes(SaltLength);
    }

    private static byte[] Hash(string pin, byte[] salt)
    {
        // Use a secure password hashing algorithm (e.g., Argon2, bcrypt, or PBKDF2)
        // Example using PBKDF2:
        using var pbkdf2 = new Rfc2898DeriveBytes(pin, salt, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashLength); // 32 bytes is a common length for a secure hash
        return hash;
    }
}