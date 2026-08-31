using System.Text.Json.Serialization;
using Shared.Model.Auth;

namespace Shared.Requests;

public abstract class ManualNetRequest
{
    public abstract bool IsWithAuthorisation { get; }

    [JsonIgnore]
    public abstract Token? AuthToken { get; }
}