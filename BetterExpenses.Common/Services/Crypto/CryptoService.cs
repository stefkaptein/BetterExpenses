using System.Security.Cryptography;
using System.Text;

namespace BetterExpenses.Common.Services.Crypto;

public interface ICryptoService
{
    public string Encrypt(string raw, string pin);
    public byte[] Encrypt(string raw, byte[] key);
    public string Decrypt(string encryptedBase64String, string pin);
    public string Decrypt(byte[] encryptedBytes, byte[] key);
}

public class CryptoService : ICryptoService
{
    public const int IvLength = 16;

    public string Encrypt(string raw, string pin) => Convert.ToBase64String(Encrypt(raw, GetKeyFromPin(pin)));
    
    public byte[] Encrypt(string raw, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        // Create the streams used for encryption.
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(raw);
        }
        
        var encrypted = msEncrypt.ToArray();
        var result = AddIvToEncryptedDate(encrypted, aes.IV);
        
        return result;
    }

    public string Decrypt(string encryptedBase64String, string pin) =>
        Decrypt(Convert.FromBase64String(encryptedBase64String), GetKeyFromPin(pin));
    
    public string Decrypt(byte[] combinedBytes, byte[] key)
    {
        var split = SplitEncryptedData(combinedBytes);

        var iv = split[0];
        var encryptedBytes = split[1];
        
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        // Create the streams used for decryption.
        using var msDecrypt = new MemoryStream(encryptedBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        return srDecrypt.ReadToEnd();
    }

    private static byte[] GetKeyFromPin(string pin)
    {
        using var hashingAlgo = SHA256.Create();

        var keyBytes = Encoding.UTF8.GetBytes(pin);
        var keyHashBytes = hashingAlgo.ComputeHash(keyBytes);
        return keyHashBytes;
    }

    private static byte[] AddIvToEncryptedDate(byte[] encryptedData, byte[] iv)
    {
        var combined = new byte[encryptedData.Length + iv.Length];
        Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, 0, combined, iv.Length, encryptedData.Length);
        return combined;
    }
    
    private static byte[][] SplitEncryptedData(byte[] encryptedData)
    {
        var encryptedLength = encryptedData.Length - IvLength;
        var result = new byte[2][];
        result[0] = new byte[IvLength];
        result[1] = new byte[encryptedLength];
        Buffer.BlockCopy(encryptedData, 0, result[0], 0, IvLength);
        Buffer.BlockCopy(encryptedData, IvLength, result[1], 0, encryptedLength);
        return result;
    }
}