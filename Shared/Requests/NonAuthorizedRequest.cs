using System.Text.Json.Serialization;
using Shared.Model.Auth;

namespace Shared.Requests;

public abstract class NonAuthorizedRequest : ManualNetRequest
{
    public override bool IsWithAuthorisation => false;
    
    [JsonIgnore]
    public override Token? AuthToken => null;
}