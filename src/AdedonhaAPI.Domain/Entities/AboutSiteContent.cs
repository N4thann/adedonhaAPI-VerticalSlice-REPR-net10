namespace AdedonhaAPI.Domain.Entities
{
    /// <summary>
    /// Texto "Sobre o site" (bio do criador) — documento único, global, editado apenas por
    /// administradores.
    /// </summary>
    public class AboutSiteContent : BaseEntity
    {
        public string Cargo { get; set; } = string.Empty;
        public List<string> Formacoes { get; set; } = new();
        public string TextoGeral { get; set; } = string.Empty;
        public List<string> Tecnologias { get; set; } = new();
        public List<string> Arquiteturas { get; set; } = new();

        /// <summary>URL pública da foto, retornada por <c>IFileStorageService</c>. <c>null</c> enquanto nenhuma imagem foi enviada.</summary>
        public string? ImageUrl { get; set; }
    }
}
