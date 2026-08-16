using AdedonhaAPI.Application.Common.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite
{
    /// <summary>Valida os campos de texto e, quando uma imagem é enviada, seu Content-Type e tamanho.</summary>
    public class UpsertAboutSiteValidator : AbstractValidator<UpsertAboutSiteInput>
    {
        public UpsertAboutSiteValidator(IOptions<FileStorageOptions> fileStorageOptions)
        {
            var options = fileStorageOptions.Value;

            RuleFor(x => x.Cargo)
                .NotEmpty().WithMessage("O cargo é obrigatório.")
                .MaximumLength(100).WithMessage("O cargo deve ter no máximo 100 caracteres.");

            RuleFor(x => x.TextoGeral)
                .NotEmpty().WithMessage("O texto geral é obrigatório.")
                .MaximumLength(5000).WithMessage("O texto geral deve ter no máximo 5000 caracteres.");

            RuleForEach(x => x.Formacoes)
                .NotEmpty().WithMessage("Um item da lista de formações não pode ser vazio.");

            RuleForEach(x => x.Tecnologias)
                .NotEmpty().WithMessage("Um item da lista de tecnologias não pode ser vazio.");

            RuleForEach(x => x.Arquiteturas)
                .NotEmpty().WithMessage("Um item da lista de arquiteturas não pode ser vazio.");

            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image!.ContentType)
                    .Must(contentType => options.AllowedImageContentTypes.Contains(contentType))
                    .WithMessage($"A imagem deve ser um dos formatos: {string.Join(", ", options.AllowedImageContentTypes)}.");

                RuleFor(x => x.Image!.Length)
                    .LessThanOrEqualTo(options.MaxFileSizeBytes)
                    .WithMessage($"A imagem deve ter no máximo {options.MaxFileSizeBytes / 1_000_000} MB.");
            });
        }
    }
}
