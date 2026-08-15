namespace AdedonhaAPI.Application.Common.Context
{
    /// <summary>
    /// Implementacao de IRequestContext para execucoes fora do pipeline HTTP (jobs em background).
    /// </summary>
    public class SystemRequestContext : IRequestContext
    {
        public string UsuarioId { get; set; } = "system";
        public string? NomeUsuario { get; set; }
        public string Origem { get; set; } = string.Empty;
    }
}
