using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.CreateWord;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class CreateWordUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IValidator<CreateWordInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<CreateWordUseCase> _loggerMock;
        private readonly CreateWordUseCase _sut;

        public CreateWordUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _validatorMock = Substitute.For<IValidator<CreateWordInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<CreateWordUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _sut = new CreateWordUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve criar a palavra sem categoria quando CategoryIds nao for informado")]
        public async Task ExecuteAsync_WhenCategoryIdsIsNull_ShouldCreateWordWithoutCategories()
        {
            // Arrange
            var input = new CreateWordInput("Adidas", null, null);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>()).Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Name.ShouldBe("Adidas");
            result.Value.Slug.ShouldBe("adidas");
            result.Value.InitialLetter.ShouldBe('A');
            result.Value.CategoryIds.ShouldBeEmpty();

            await _wordRepoMock.Received(1).AddAsync(Arg.Is<Word>(w => w.Categories.Count == 0), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "SUCESSO - Deve criar a palavra ja associada as categorias informadas")]
        public async Task ExecuteAsync_WhenCategoryIdsAreValid_ShouldCreateWordWithCategories()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            var input = new CreateWordInput("Adidas", "Marca esportiva", new List<string> { category.Id });

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>()).Returns(new List<Word>());
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.CategoryIds.ShouldHaveSingleItem();

            await _wordRepoMock.Received(1).AddAsync(Arg.Is<Word>(w =>
                w.Categories.Count == 1 && w.Categories[0].CategoryId == category.Id
            ), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando alguma categoria informada nao existir")]
        public async Task ExecuteAsync_WhenSomeCategoryIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var input = new CreateWordInput("Adidas", null, new List<string> { "id-inexistente" });
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>()).Returns(new List<Word>());
            _categoryRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);

            await _wordRepoMock.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Conflict quando ja existir palavra com o mesmo nome")]
        public async Task ExecuteAsync_WhenNameAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var input = new CreateWordInput("Adidas", null, null);
            Word existing = WordDataBuilder.Create().WithName("Adidas");

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>()).Returns(new List<Word> { existing });

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("Word.Conflict.NameAlreadyExists");
        }
    }
}
