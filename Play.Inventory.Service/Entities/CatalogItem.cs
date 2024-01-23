using MongoDB.Bson.Serialization.Attributes;
using Play.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Play.Inventory.Service.Entities
{
    public class CatalogItem : IEntity
    {
        [Key, Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
