using System.Text.Json;

namespace AdedonhaAPI.Extensions
{
    /// <summary>
    /// Corpo de resposta padrão devolvido pelo middleware global de exceções.
    /// </summary>
    public class ErrorDetailsOutput
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Trace { get; set; }
        public string? OperationId { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
