using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AdedonhaAPI.Domain.Entities
{
    /// <summary>
    /// Campos comuns a toda entidade persistida no MongoDB: identificador, data de criação e flag de ativação lógica.
    /// </summary>
    public abstract class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; } = ObjectId.GenerateNewId().ToString();

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
