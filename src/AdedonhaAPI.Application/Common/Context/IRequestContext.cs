namespace AdedonhaAPI.Application.Common.Context
{
    /// <summary>
    /// Contexto de "quem" e "onde" para a execucao atual, populado uma unica vez por requisicao.
    /// </summary>
    public interface IRequestContext
    {
        string UsuarioId { get; }
        string? NomeUsuario { get; }
        string Origem { get; }
    }
}
