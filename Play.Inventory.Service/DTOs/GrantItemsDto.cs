namespace Play.Inventory.Service.DTOs
{
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
}
