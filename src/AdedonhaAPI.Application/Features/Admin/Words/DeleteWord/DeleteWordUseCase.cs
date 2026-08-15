using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.DeleteWord
{
    /// <summary>
    /// Soft delete de palavra.
    /// </summary>
    public class DeleteWordUseCase : IUseCase<DeleteWordInput, ErrorOr<DeleteWordOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<DeleteWordUseCase> _logger;

        public DeleteWordUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<DeleteWordUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<DeleteWordOutput>> ExecuteAsync(DeleteWordInput input, CancellationToken cancellationToken)
        {
            var word = await _unitOfWork.Words.GetByIdAsync(input.Id, cancellationToken);
            if (word is null || !word.IsActive)
                return Error.NotFound("Word.NotFound", "Palavra não encontrada.");

            word.IsActive = false;
            await _unitOfWork.Words.UpdateAsync(word, cancellationToken);

            _logger.LogInfo("Palavra removida (soft delete)", _requestContext, new() { ["WordId"] = word.Id });

            return new DeleteWordOutput(word.Id);
        }
    }
}
