using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Shared.Domain
{
    public class Word
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] 
        public Guid Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Slug")]
        public string Slug { get; set; }

        [BsonElement("InitialLetter")]
        public char InitialLetter { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("Categories")]
        public List<CategoryInfo> Categories { get; set; } = new();

        public class CategoryInfo
        {
            [BsonRepresentation(BsonType.String)]
            public Guid CategoryId { get; set; }

            [BsonElement("Slug")]
            public string Slug { get; set; }

            [BsonElement("Name")]
            public string Name { get; set; }
        }
    }
}
