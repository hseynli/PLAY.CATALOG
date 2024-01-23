namespace Play.Inventory.Service.DTOs
{
    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTime AcquiredDate);
}