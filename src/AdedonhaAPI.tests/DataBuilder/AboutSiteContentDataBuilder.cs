using AdedonhaAPI.Domain.Entities;
using Bogus;

namespace AdedonhaAPI.tests.DataBuilder
{
    /// <summary>Gera textos "Sobre o site" (bio do criador, Bogus) para testes.</summary>
    public class AboutSiteContentDataBuilder
    {
        private readonly AboutSiteContent _instance;

        public AboutSiteContentDataBuilder()
        {
            var faker = new Faker<AboutSiteContent>("pt_BR")
                .RuleFor(a => a.Cargo, f => f.Name.JobTitle())
                .RuleFor(a => a.Formacoes, f => new List<string> { f.Name.JobTitle(), f.Name.JobTitle() })
                .RuleFor(a => a.TextoGeral, f => f.Lorem.Paragraph())
                .RuleFor(a => a.Tecnologias, f => new List<string> { f.Hacker.Noun(), f.Hacker.Noun() })
                .RuleFor(a => a.Arquiteturas, f => new List<string> { f.Hacker.Abbreviation(), f.Hacker.Abbreviation() })
                .RuleFor(a => a.ImageUrl, f => f.Internet.UrlRootedPath() + "/foto.jpg");

            _instance = faker.Generate();
        }

        public static AboutSiteContentDataBuilder Create() => new();
        public AboutSiteContent Build() => _instance;
        public static implicit operator AboutSiteContent(AboutSiteContentDataBuilder builder) => builder.Build();

        public AboutSiteContentDataBuilder WithCargo(string cargo)
        {
            _instance.Cargo = cargo;
            return this;
        }

        public AboutSiteContentDataBuilder WithFormacoes(List<string> formacoes)
        {
            _instance.Formacoes = formacoes;
            return this;
        }

        public AboutSiteContentDataBuilder WithTextoGeral(string textoGeral)
        {
            _instance.TextoGeral = textoGeral;
            return this;
        }

        public AboutSiteContentDataBuilder WithTecnologias(List<string> tecnologias)
        {
            _instance.Tecnologias = tecnologias;
            return this;
        }

        public AboutSiteContentDataBuilder WithArquiteturas(List<string> arquiteturas)
        {
            _instance.Arquiteturas = arquiteturas;
            return this;
        }

        public AboutSiteContentDataBuilder WithImageUrl(string? imageUrl)
        {
            _instance.ImageUrl = imageUrl;
            return this;
        }
    }
}
