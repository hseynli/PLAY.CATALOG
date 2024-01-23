namespace Play.Catalog.Service.DTOs
{
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTime CreatedDate);
}
