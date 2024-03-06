using Bunq.Sdk.Model.Core;
using Newtonsoft.Json;

namespace BetterExpenses.Common.Services.Bunq.Models;

public class Attachment : BunqModel
{
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "content_type")]
    public string ContentType { get; set; }

    [JsonProperty(PropertyName = "urls")]
    public List<AttachmentUrl> Urls { get; set; }
    
    public override bool IsAllFieldNull() => this.Description == null && this.ContentType == null;

    public static Attachment CreateFromJsonString(string json)
    {
        return BunqModel.CreateFromJsonString<Attachment>(json);
    }
}