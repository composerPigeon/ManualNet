using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Server.Controllers.Requests;
using Server.Data.EntityContexts;

namespace Server.Model.Auth;

public class ApplicationUser : IdentityUser
{
    [MaxLength(ApplicationUserContext.NameMaxLength)]
    public string FirstName { get; init; } = string.Empty;

    [MaxLength(ApplicationUserContext.NameMaxLength)]
    public string LastName { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
    public DateTime LastLoginAt { get; set; }

    public static ApplicationUser From(RegisterRequest request)
    {
        return new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };
    }
}
