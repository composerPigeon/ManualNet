namespace Server.Model.Auth;

public readonly struct HashToken 
{
    public string Value { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string Hash { get; init; }

    private Token AsToken()
    {
        return new Token
        {
            Value = Value,
            ExpiresAt = ExpiresAt,
        };
    }
    
    public static implicit operator Token(HashToken hashToken)
    {
        return hashToken.AsToken();
    }
}
