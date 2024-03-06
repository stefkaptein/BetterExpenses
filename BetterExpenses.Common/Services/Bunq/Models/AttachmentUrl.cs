using Bunq.Sdk.Model.Core;
using Newtonsoft.Json;

namespace BetterExpenses.Common.Services.Bunq.Models;

public class AttachmentUrl : BunqModel
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }
    
    [JsonProperty(PropertyName = "url")]
    public string Url { get; set; }
    
    public override bool IsAllFieldNull() => this.Type == null && this.Url == null;

    public static Attachment CreateFromJsonString(string json)
    {
        return BunqModel.CreateFromJsonString<Attachment>(json);
    }
}