using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Features.Admin.Words.BulkUploadWordsCsv;
using AdedonhaAPI.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Infrastructure.Database
{
    /// <summary>
    /// Faz o seed inicial de categorias/palavras a partir do adedonha_palavras.csv na primeira
    /// subida do app (coleção de palavras vazia). Idempotente — não faz nada em subidas seguintes.
    /// </summary>
    public class WordsSeederService : IHostedService
    {
        private const string CsvFileName = "adedonha_palavras.csv";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WordsSeederService> _logger;

        public WordsSeederService(IServiceProvider serviceProvider, ILogger<WordsSeederService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var systemContext = new SystemRequestContext { UsuarioId = "system", Origem = "WordsSeederService" };

            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var existingWord = await unitOfWork.Words.GetFirstOrDefaultAsync(
                filter: null, orderBy: w => w.Id, ascending: true, cancellationToken);

            if (existingWord is not null)
            {
                _logger.LogInfo("Seed de palavras já existe, seed ignorado", systemContext);
                return;
            }

            var csvPath = Path.Combine(AppContext.BaseDirectory, "Data", CsvFileName);
            var lines = await File.ReadAllLinesAsync(csvPath, cancellationToken);

            var useCaseLogger = scope.ServiceProvider.GetRequiredService<ILogger<BulkUploadWordsCsvUseCase>>();
            var useCase = new BulkUploadWordsCsvUseCase(unitOfWork, systemContext, useCaseLogger);

            var result = await useCase.ExecuteAsync(new BulkUploadWordsCsvInput(lines), cancellationToken);
            var output = result.Value;

            _logger.LogInfo("Seed de palavras concluído", systemContext, new()
            {
                ["TotalLinhas"] = output.TotalRows,
                ["CategoriasCriadas"] = output.CategoriesCreated,
                ["PalavrasCriadas"] = output.WordsCreated,
                ["AssociacoesCriadas"] = output.AssociationsCreated,
                ["LinhasPuladas"] = output.RowsSkipped,
                ["LinhasComErro"] = output.Errors.Count
            });
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
