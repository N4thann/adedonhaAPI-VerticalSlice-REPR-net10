using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryWordCounts
{
    /// <summary>
    /// Conta palavras ativas por categoria ativa, para o grafico donut do dashboard Admin.
    /// </summary>
    public class GetCategoryWordCountsUseCase : IUseCase<GetCategoryWordCountsInput, ErrorOr<GetCategoryWordCountsOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCategoryWordCountsUseCase> _logger;

        public GetCategoryWordCountsUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCategoryWordCountsUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCategoryWordCountsOutput>> ExecuteAsync(GetCategoryWordCountsInput input, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Calculando contagem de palavras por categoria", _requestContext);

            var categories = await _unitOfWork.Categories.FindAsync(c => c.IsActive, cancellationToken);
            var words = await _unitOfWork.Words.FindAsync(w => w.IsActive, cancellationToken);

            var items = categories
                .Select(c => new CategoryWordCount(c.Name, c.Slug, words.Count(w => w.Categories.Any(ci => ci.CategoryId == c.Id))))
                .Where(c => c.WordCount > 0)
                .OrderByDescending(c => c.WordCount)
                .ToList();

            return new GetCategoryWordCountsOutput(items);
        }
    }
}
