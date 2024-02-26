using BetterExpenses.Common.Services.Crypto;

namespace BetterExpensesAPI.Test.Crypto;

public class EncryptApiKeyTest
{
    [Test]
    public void TestSimpleEncryption()
    {
        var pin = "Test12345";
        var apiKey = "hJ7lE9wPfK5oZnX3qIvYsG4rD";

        var cryptoService = new CryptoService();
        
        var encryptedResult = cryptoService.Encrypt(apiKey, pin);
        var decryptResult = cryptoService.Decrypt(encryptedResult, pin);

        Assert.That(decryptResult, Is.EqualTo(apiKey));
    }
}