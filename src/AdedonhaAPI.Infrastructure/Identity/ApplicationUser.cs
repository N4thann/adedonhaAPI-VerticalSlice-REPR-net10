using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Infrastructure.Identity
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("RefreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("RefreshTokenExpiryTime")]
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
