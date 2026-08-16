using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogAboutSite
{
    /// <summary>Sem parâmetros — "Sobre o site" é um documento único e global, não depende de categoria nem de usuário logado.</summary>
    public record GetCatalogAboutSiteInput() : IInput<ErrorOr<GetCatalogAboutSiteOutput>>;
}
