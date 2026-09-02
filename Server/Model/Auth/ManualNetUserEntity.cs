using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Server.Data.EntityContexts;
using Server.Model.Domain;
using Shared.Model.Auth;
using Shared.Requests;

namespace Server.Model.Auth;

public class ManualNetUserEntity : IdentityUser, IManualNetUser, IEntityBase<string>
{
    [MaxLength(IEntityContext.MaxNameLength)]
    public string FirstName { get; private init; } = string.Empty;

    [MaxLength(IEntityContext.MaxNameLength)]
    public string LastName { get; private init; } = string.Empty;

    public DateTime CreatedAt { get; private init; }
    public DateTime LastLoginAt { get; private set; }
    
    public new ManualNetEmail Email
    {
        get => base.Email;
        private init => base.Email = value.ToString();
    }

    public override string ToString()
    {
        return $"User(email: {Email}, id: {Id})";
    }
    public void Login(DateTime loginAt)
    {
        LastLoginAt = loginAt;
    }

    public static ManualNetUserEntity From(RegisterRequest request)
    {
        return new ManualNetUserEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email.ToString(),
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };
    }
}
