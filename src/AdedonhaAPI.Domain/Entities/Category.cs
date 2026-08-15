using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Domain.Entities
{
    public class Category : BaseEntity
    {
        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("Slug")]
        public string Slug { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonElement("IconUrl")]
        public string? IconUrl { get; set; }
    }
}
