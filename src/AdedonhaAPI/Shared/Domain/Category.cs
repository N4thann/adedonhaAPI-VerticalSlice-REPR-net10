using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Shared.Domain
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Slug")]
        public string Slug { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("IconUrl")]
        public string IconUrl { get; set; }
    }
}
