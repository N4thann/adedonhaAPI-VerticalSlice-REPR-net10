using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.BulkUploadWordsCsv;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class BulkUploadWordsCsvUseCaseTests
    {
        private readonly List<Category> _categories = new();
        private readonly List<Word> _words = new();
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<BulkUploadWordsCsvUseCase> _loggerMock;
        private readonly BulkUploadWordsCsvUseCase _sut;

        public BulkUploadWordsCsvUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<BulkUploadWordsCsvUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);

            // Repositorios "fake" apoiados em listas em memoria -- necessario porque o UseCase
            // consulta e grava varias vezes ao longo do processamento linha a linha do CSV.
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(ci => _categories.Where(((Expression<Func<Category, bool>>)ci[0]).Compile()).ToList().AsEnumerable());
            _categoryRepoMock.AddAsync(Arg.Do<Category>(c => _categories.Add(c)), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(ci => _words.Where(((Expression<Func<Word, bool>>)ci[0]).Compile()).ToList().AsEnumerable());
            _wordRepoMock.AddAsync(Arg.Do<Word>(w => _words.Add(w)), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);
            _wordRepoMock.UpdateAsync(Arg.Any<Word>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _sut = new BulkUploadWordsCsvUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve criar categoria e palavra novas e associa-las")]
        public async Task ExecuteAsync_WhenCsvHasNewCategoryAndWord_ShouldCreateAndAssociate()
        {
            // Arrange
            var lines = new List<string> { "Id,Categoria,Letra,Palavra", "1,Marca,A,Adidas" };

            // Act
            var result = await _sut.ExecuteAsync(new BulkUploadWordsCsvInput(lines), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalRows.ShouldBe(1);
            result.Value.CategoriesCreated.ShouldBe(1);
            result.Value.WordsCreated.ShouldBe(1);
            result.Value.AssociationsCreated.ShouldBe(1);
            result.Value.RowsSkipped.ShouldBe(0);
            result.Value.Errors.ShouldBeEmpty();

            _categories.ShouldHaveSingleItem();
            _words.ShouldHaveSingleItem();
            _words[0].Categories.ShouldHaveSingleItem();
        }

        [Fact(DisplayName = "SUCESSO - Deve reaproveitar categoria/palavra existentes e pular linha duplicada")]
        public async Task ExecuteAsync_WhenRowIsAlreadyProcessed_ShouldSkipDuplicate()
        {
            // Arrange
            var lines = new List<string>
            {
                "Id,Categoria,Letra,Palavra",
                "1,Marca,A,Adidas",
                "2,Marca,A,Adidas"
            };

            // Act
            var result = await _sut.ExecuteAsync(new BulkUploadWordsCsvInput(lines), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalRows.ShouldBe(2);
            result.Value.CategoriesCreated.ShouldBe(1);
            result.Value.WordsCreated.ShouldBe(1);
            result.Value.AssociationsCreated.ShouldBe(1);
            result.Value.RowsSkipped.ShouldBe(1);

            _categories.ShouldHaveSingleItem();
            _words.ShouldHaveSingleItem();
        }

        [Fact(DisplayName = "SUCESSO - Deve reportar linha malformada como erro sem abortar o processamento das demais")]
        public async Task ExecuteAsync_WhenRowIsMalformed_ShouldReportErrorAndContinue()
        {
            // Arrange
            var lines = new List<string>
            {
                "Id,Categoria,Letra,Palavra",
                "1,Marca,A",
                "2,Marca,A,Nike"
            };

            // Act
            var result = await _sut.ExecuteAsync(new BulkUploadWordsCsvInput(lines), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Errors.ShouldHaveSingleItem();
            result.Value.Errors[0].Line.ShouldBe(2);
            result.Value.WordsCreated.ShouldBe(1);

            _words.ShouldHaveSingleItem();
            _words[0].Name.ShouldBe("Nike");
        }
    }
}
