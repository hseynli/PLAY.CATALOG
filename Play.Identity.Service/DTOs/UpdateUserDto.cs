using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs
{
    public record UpdateUserDto([Required][EmailAddress] string Email, [Range(0, 1_000_000)] decimal Gil);
}
