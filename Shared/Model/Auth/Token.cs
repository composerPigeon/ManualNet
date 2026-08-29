namespace Shared.Model.Auth;

public readonly struct Token
{
    public string Value { get; init; }
    public DateTime ExpiresAt { get; init; }
}