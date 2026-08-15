using AdedonhaAPI.Application.Common.Context;

namespace AdedonhaAPI.Common.Context
{
    public class RequestContext : IRequestContext
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string? NomeUsuario { get; set; }
        public string Origem { get; set; } = string.Empty;
    }
}
