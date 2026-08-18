using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWordStats
{
    /// <summary>
    /// Calcula o total de palavras ativas, para o dashboard Admin.
    /// </summary>
    public class GetWordStatsUseCase : IUseCase<GetWordStatsInput, ErrorOr<GetWordStatsOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetWordStatsUseCase> _logger;

        public GetWordStatsUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetWordStatsUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetWordStatsOutput>> ExecuteAsync(GetWordStatsInput input, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Calculando estatisticas de palavras", _requestContext);

            var words = await _unitOfWork.Words.FindAsync(w => w.IsActive, cancellationToken);

            return new GetWordStatsOutput(words.Count());
        }
    }
}
