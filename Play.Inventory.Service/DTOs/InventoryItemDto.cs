namespace Play.Inventory.Service.DTOs
{
    public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTime AcquiredDate);
}