namespace Shared.Model.Auth;

public class ManualNetUserDto : IEntityDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public ManualNetEmail Email { get; init; }
}