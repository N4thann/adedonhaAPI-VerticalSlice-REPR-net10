using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Common.Storage;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite
{
    /// <summary>
    /// Cria ou atualiza (find-or-create) o texto "Sobre o site". Restrito a administradores.
    /// Quando uma imagem nova é enviada, a anterior (se houver) é apagada do armazenamento.
    /// </summary>
    public class UpsertAboutSiteUseCase : IUseCase<UpsertAboutSiteInput, ErrorOr<UpsertAboutSiteOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IValidator<UpsertAboutSiteInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<UpsertAboutSiteUseCase> _logger;

        public UpsertAboutSiteUseCase(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            IValidator<UpsertAboutSiteInput> validator,
            IRequestContext requestContext,
            ILogger<UpsertAboutSiteUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<UpsertAboutSiteOutput>> ExecuteAsync(UpsertAboutSiteInput input, CancellationToken cancellationToken)
        {
            _logger.LogBegin("Cadastro do texto Sobre o site", _requestContext);

            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Cadastro do texto Sobre o site rejeitado por falha de validação", _requestContext,
                    new() { ["Campos"] = validationResult.Errors.Select(e => e.PropertyName) });
                _logger.LogEnd("Cadastro do texto Sobre o site", _requestContext);
                return validationResult.Errors
                    .Select(v => Error.Validation(code: v.PropertyName, description: v.ErrorMessage))
                    .ToList();
            }

            var existing = (await _unitOfWork.AboutSite.GetAllAsync(cancellationToken)).FirstOrDefault();

            var imageUrl = existing?.ImageUrl;

            if (input.Image is not null)
            {
                imageUrl = await _fileStorageService.SaveAsync(input.Image, cancellationToken);

                if (existing?.ImageUrl is not null)
                    await _fileStorageService.DeleteAsync(existing.ImageUrl, cancellationToken);
            }

            if (existing is null)
            {
                var created = new AboutSiteContent
                {
                    Cargo = input.Cargo,
                    Formacoes = input.Formacoes,
                    TextoGeral = input.TextoGeral,
                    Tecnologias = input.Tecnologias,
                    Arquiteturas = input.Arquiteturas,
                    ImageUrl = imageUrl
                };
                await _unitOfWork.AboutSite.AddAsync(created, cancellationToken);
            }
            else
            {
                existing.Cargo = input.Cargo;
                existing.Formacoes = input.Formacoes;
                existing.TextoGeral = input.TextoGeral;
                existing.Tecnologias = input.Tecnologias;
                existing.Arquiteturas = input.Arquiteturas;
                existing.ImageUrl = imageUrl;
                await _unitOfWork.AboutSite.UpdateAsync(existing, cancellationToken);
            }

            _logger.LogEnd("Cadastro do texto Sobre o site", _requestContext);

            return new UpsertAboutSiteOutput(input.Cargo, input.Formacoes, input.TextoGeral, input.Tecnologias, input.Arquiteturas, imageUrl);
        }
    }
}
