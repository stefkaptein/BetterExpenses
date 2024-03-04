using BetterExpenses.Common.Options;
using Bunq.Sdk.Context;
using Microsoft.Extensions.Options;

namespace BetterExpenses.Common.Services.Crypto;

public interface IApiContextCryptoFileService
{
    public Task SaveApiContext(ApiContext apiContext, Guid userId);
    public Task<ApiContext> LoadApiContext(Guid userId);
    public void DeleteApiContext(Guid userId);
}

public class ApiContextCryptoFileService(
    ICryptoService cryptoService,
    IOptions<CryptoOptions> cryptoOptions,
    IOptions<ApiContextFileOptions> fileOptions) : IApiContextCryptoFileService
{
    private readonly ICryptoService _cryptoService = cryptoService;
    private readonly byte[] _key = Convert.FromBase64String(cryptoOptions.Value.ConfigFileEncryptionKey);
    private readonly string _path = fileOptions.Value.Path;

    public async Task SaveApiContext(ApiContext apiContext, Guid userId)
    {
        var contextJsonString = apiContext.ToJson();
        var encrypted = _cryptoService.Encrypt(contextJsonString, _key);
        var filePath = Path.Join(_path, userId.ToString());
        await File.WriteAllBytesAsync(filePath, encrypted);
    }

    public async Task<ApiContext> LoadApiContext(Guid userId)
    {
        var filePath = Path.Join(_path, userId.ToString());
        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var contextJsonString = _cryptoService.Decrypt(fileBytes, _key);
        return ApiContext.FromJson(contextJsonString);
    }

    public void DeleteApiContext(Guid userId)
    {
        var filePath = Path.Join(_path, userId.ToString());
        File.Delete(filePath);
    }
}