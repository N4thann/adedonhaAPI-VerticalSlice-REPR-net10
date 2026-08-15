using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.BulkUploadWordsCsv
{
    /// <summary>
    /// Processa um CSV no formato Id,Categoria,Letra,Palavra: cria categorias/palavras
    /// inexistentes por find-or-create e associa palavra a categoria, pulando duplicatas
    /// e reportando linhas malformadas sem abortar o processamento das demais.
    /// </summary>
    public class BulkUploadWordsCsvUseCase : IUseCase<BulkUploadWordsCsvInput, ErrorOr<BulkUploadWordsCsvOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<BulkUploadWordsCsvUseCase> _logger;

        public BulkUploadWordsCsvUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<BulkUploadWordsCsvUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<BulkUploadWordsCsvOutput>> ExecuteAsync(BulkUploadWordsCsvInput input, CancellationToken cancellationToken)
        {
            _logger.LogBegin("Upload em massa de palavras via CSV", _requestContext, new() { ["TotalLinhas"] = input.Lines.Count });

            var dataLines = input.Lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            var categoriesCreated = 0;
            var wordsCreated = 0;
            var associationsCreated = 0;
            var rowsSkipped = 0;
            var errors = new List<BulkUploadRowError>();

            for (var i = 0; i < dataLines.Count; i++)
            {
                var lineNumber = i + 2;
                var columns = dataLines[i].Split(',');

                if (columns.Length < 4)
                {
                    errors.Add(new BulkUploadRowError(lineNumber, "Linha com colunas insuficientes (esperado: Id,Categoria,Letra,Palavra)."));
                    continue;
                }

                var categoryName = columns[1].Trim();
                var wordName = columns[3].Trim();

                if (string.IsNullOrWhiteSpace(categoryName) || string.IsNullOrWhiteSpace(wordName))
                {
                    errors.Add(new BulkUploadRowError(lineNumber, "Categoria ou Palavra vazia."));
                    continue;
                }

                var categorySlug = SlugGenerator.Generate(categoryName);
                var category = (await _unitOfWork.Categories.FindAsync(c => c.Slug == categorySlug, cancellationToken)).FirstOrDefault();
                if (category is null)
                {
                    category = new Category { Name = categoryName, Slug = categorySlug };
                    await _unitOfWork.Categories.AddAsync(category, cancellationToken);
                    categoriesCreated++;
                }

                var wordSlug = SlugGenerator.Generate(wordName);
                var word = (await _unitOfWork.Words.FindAsync(w => w.Slug == wordSlug, cancellationToken)).FirstOrDefault();
                if (word is null)
                {
                    word = new Word { Name = wordName, Slug = wordSlug, InitialLetter = SlugGenerator.GetInitialLetter(wordName) };
                    await _unitOfWork.Words.AddAsync(word, cancellationToken);
                    wordsCreated++;
                }

                if (word.Categories.Any(c => c.CategoryId == category.Id))
                {
                    rowsSkipped++;
                    continue;
                }

                word.Categories.Add(new Word.CategoryInfo { CategoryId = category.Id, Slug = category.Slug, Name = category.Name });
                await _unitOfWork.Words.UpdateAsync(word, cancellationToken);
                associationsCreated++;
            }

            _logger.LogEnd("Upload em massa de palavras via CSV", _requestContext, new()
            {
                ["CategoriasCriadas"] = categoriesCreated,
                ["PalavrasCriadas"] = wordsCreated,
                ["AssociacoesCriadas"] = associationsCreated,
                ["LinhasPuladas"] = rowsSkipped,
                ["LinhasComErro"] = errors.Count
            });

            return new BulkUploadWordsCsvOutput(dataLines.Count, categoriesCreated, wordsCreated, associationsCreated, rowsSkipped, errors);
        }
    }
}
