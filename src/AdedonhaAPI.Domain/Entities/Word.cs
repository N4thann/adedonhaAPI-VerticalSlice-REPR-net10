using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Domain.Entities
{
    public class Word : BaseEntity
    {
        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("Slug")]
        public string Slug { get; set; } = string.Empty;

        [BsonElement("InitialLetter")]
        public char InitialLetter { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonElement("Categories")]
        public List<CategoryInfo> Categories { get; set; } = new();

        public class CategoryInfo
        {
            [BsonElement("CategoryId")]
            public string CategoryId { get; set; } = string.Empty;

            [BsonElement("Slug")]
            public string Slug { get; set; } = string.Empty;

            [BsonElement("Name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
