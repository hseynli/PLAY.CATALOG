namespace Play.Identity.Service.DTOs
{
    public record UserDto(Guid Id, string Username, string Email, decimal Gil, DateTime CreatedDate);
}
