using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Shared.Identity
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        [BsonElement("RefreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("RefreshTokenExpiryTime")]
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
