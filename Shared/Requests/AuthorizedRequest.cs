using System.Text.Json.Serialization;
using Shared.Model.Auth;

namespace Shared.Requests;

public class AuthorizedRequest(Token authToken) : ManualNetRequest
{
    public override bool IsWithAuthorisation => true;
    
    [JsonIgnore]
    public override Token? AuthToken => authToken;
}