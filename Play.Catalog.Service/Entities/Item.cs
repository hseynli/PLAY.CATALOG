using MongoDB.Bson.Serialization.Attributes;
using Play.Common;
using Play.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Entities
{
    public class Item : IEntity
    {
        public Item(Guid id, string name, string description, decimal price, DateTime createdDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CreatedDate = createdDate;
        }

        [Key, Required]
        public Guid Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Description")]
        public string Description { get; set; }
        [BsonElement("Price")]
        public decimal Price { get; set; }
        [BsonElement("CreatedDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }
        public string SomeText { get; set; } = "Hello, World!";
    }
}