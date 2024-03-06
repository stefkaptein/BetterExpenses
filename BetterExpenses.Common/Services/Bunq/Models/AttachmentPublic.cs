using Bunq.Sdk.Http;
using Bunq.Sdk.Model.Core;
using Newtonsoft.Json;

namespace BetterExpenses.Common.Services.Bunq.Models;

public class AttachmentPublic : BunqModel
{
    [JsonProperty(PropertyName = "uuid")]
    public string Uuid { get; set; }

    [JsonProperty(PropertyName = "created")]
    public string Created { get; set; }

    [JsonProperty(PropertyName = "updated")]
    public string Updated { get; set; }

    [JsonProperty(PropertyName = "attachment")]
    public Attachment Attachment { get; set; }

    public override bool IsAllFieldNull()
    {
        return this.Uuid == null && this.Created == null && this.Updated == null && this.Attachment == null;
    }

    public static AttachmentPublic CreateFromJsonString(string json)
    {
        return BunqModel.CreateFromJsonString<AttachmentPublic>(json);
    }
}