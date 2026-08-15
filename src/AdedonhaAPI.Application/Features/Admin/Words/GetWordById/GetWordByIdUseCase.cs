using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWordById
{
    /// <summary>
    /// Busca uma palavra ativa pelo Id.
    /// </summary>
    public class GetWordByIdUseCase : IUseCase<GetWordByIdInput, ErrorOr<GetWordByIdOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetWordByIdUseCase> _logger;

        public GetWordByIdUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetWordByIdUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetWordByIdOutput>> ExecuteAsync(GetWordByIdInput input, CancellationToken cancellationToken)
        {
            var word = await _unitOfWork.Words.GetByIdAsync(input.Id, cancellationToken);
            if (word is null || !word.IsActive)
            {
                _logger.LogWarning("Palavra não encontrada", _requestContext, new() { ["WordId"] = input.Id });
                return Error.NotFound("Word.NotFound", "Palavra não encontrada.");
            }

            return new GetWordByIdOutput(word.Id, word.Name, word.Slug, word.InitialLetter, word.Description, word.Categories.Select(c => c.CategoryId).ToList());
        }
    }
}
