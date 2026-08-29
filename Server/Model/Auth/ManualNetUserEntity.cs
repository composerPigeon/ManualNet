using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Server.Data.EntityContexts;
using Shared.Model.Auth;
using Shared.Requests;

namespace Server.Model.Auth;

public class ManualNetUserEntity : IdentityUser, IManualNetUser, IEntityBase<string>
{
    [MaxLength(ManualNetUserEntityContext.NameMaxLength)]
    public string FirstName { get; private init; } = string.Empty;

    [MaxLength(ManualNetUserEntityContext.NameMaxLength)]
    public string LastName { get; private init; } = string.Empty;

    public DateTime CreatedAt { get; private init; }
    public DateTime LastLoginAt { get; set; }

    [MaxLength(ManualNetUserEntityContext.EmailMaxLength)]
    public new Email Email
    {
        get => base.Email;
        private init => base.Email = value.ToString();
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
