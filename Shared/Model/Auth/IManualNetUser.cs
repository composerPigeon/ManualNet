namespace Shared.Model.Auth;

public interface IManualNetUser
{
    public string Id { get; }
    
    public string FirstName { get; }
    public string LastName { get; }
    public Email Email { get; }
}