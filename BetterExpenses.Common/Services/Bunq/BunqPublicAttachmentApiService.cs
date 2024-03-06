using System.Text;
using BetterExpenses.Common.Services.Bunq.Models;
using BetterExpenses.Common.Services.Context;

namespace BetterExpenses.Common.Services.Bunq;

public interface IBunqPublicAttachmentApiService
{
    public string GetPublicAttachmentUrl(Guid userId, string uuid);
}

public class BunqPublicAttachmentApiService(IApiContextService contextService)
    : BunqApiService(contextService), IBunqPublicAttachmentApiService
{
    public string GetPublicAttachmentUrl(Guid userId, string uuid)
    {
        GetClientAndUserId(userId, out var apiClient, out var _);

        var url = $"attachment-public/{uuid}";

        var responseRaw = apiClient.Get(url, EmptyParameters, EmptyParameters);
        var rawString = Encoding.UTF8.GetString(responseRaw.BodyBytes);
        var attachmentPublic = FromJson<AttachmentPublic>(responseRaw, nameof(AttachmentPublic)).Value;
        return attachmentPublic.Attachment.Urls.First().Url;
    }
}