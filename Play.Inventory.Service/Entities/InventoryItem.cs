using MongoDB.Bson.Serialization.Attributes;
using Play.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Play.Inventory.Service.Entities
{
    public class InventoryItem : IEntity
    {
        [Key, Required]
        public Guid Id { get; set; }
        [BsonElement("userId")]
        public Guid UserId { get; set; }
        [BsonElement("catalogItemId")]
        public Guid CatalogItemId { get; set; }
        [BsonElement("quantity")]
        public int Quantity { get; set; }
        [BsonElement("acquiredDate")]
        public DateTime AcquiredDate { get; set; }
    }
}
