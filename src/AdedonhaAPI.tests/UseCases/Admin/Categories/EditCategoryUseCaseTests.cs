using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Categories.EditCategory;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Categories
{
    public class EditCategoryUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IValidator<EditCategoryInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<EditCategoryUseCase> _loggerMock;
        private readonly EditCategoryUseCase _sut;

        public EditCategoryUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _validatorMock = Substitute.For<IValidator<EditCategoryInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<EditCategoryUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new EditCategoryUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve atualizar a categoria e cascatear o novo nome/slug para as palavras associadas")]
        public async Task ExecuteAsync_WhenNameChanges_ShouldUpdateCategoryAndCascadeToWords()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithName("Animais");
            var input = new EditCategoryInput(category.Id, "Bichos", "Nova descrição");

            Word affectedWord = WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo>
            {
                new() { CategoryId = category.Id, Slug = category.Slug, Name = category.Name }
            });

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category>());
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word> { affectedWord });

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Name.ShouldBe("Bichos");
            result.Value.Slug.ShouldBe("bichos");

            await _categoryRepoMock.Received(1).UpdateAsync(Arg.Is<Category>(c => c.Name == "Bichos" && c.Slug == "bichos"), Arg.Any<CancellationToken>());
            await _wordRepoMock.Received(1).UpdateAsync(Arg.Is<Word>(w =>
                w.Categories.Single(c => c.CategoryId == category.Id).Name == "Bichos" &&
                w.Categories.Single(c => c.CategoryId == category.Id).Slug == "bichos"
            ), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria nao existir")]
        public async Task ExecuteAsync_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var input = new EditCategoryInput("id-inexistente", "Bichos", null);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);

            await _wordRepoMock.DidNotReceiveWithAnyArgs().FindAsync(default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Conflict quando o novo nome ja pertencer a outra categoria")]
        public async Task ExecuteAsync_WhenNewNameBelongsToAnotherCategory_ShouldReturnConflict()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithName("Animais");
            Category otherCategory = CategoryDataBuilder.Create().WithName("Bichos");
            var input = new EditCategoryInput(category.Id, "Bichos", null);

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { otherCategory });

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);

            await _categoryRepoMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }
    }
}
