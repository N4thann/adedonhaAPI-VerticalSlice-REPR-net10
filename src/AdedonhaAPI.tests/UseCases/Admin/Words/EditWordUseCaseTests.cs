using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.EditWord;
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
    public class EditWordUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IValidator<EditWordInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<EditWordUseCase> _loggerMock;
        private readonly EditWordUseCase _sut;

        public EditWordUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _validatorMock = Substitute.For<IValidator<EditWordInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<EditWordUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new EditWordUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve atualizar Name/Description e regenerar Slug/InitialLetter sem alterar Categories")]
        public async Task ExecuteAsync_WhenNameChanges_ShouldUpdateWordWithoutTouchingCategories()
        {
            // Arrange
            var existingCategories = new List<Word.CategoryInfo> { new() { CategoryId = "cat-1", Slug = "marca", Name = "Marca" } };
            Word word = WordDataBuilder.Create().WithName("Adidas").WithCategories(existingCategories);
            var input = new EditWordInput(word.Id, "Adidas Originals", "Nova descrição");

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>()).Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Name.ShouldBe("Adidas Originals");
            result.Value.Slug.ShouldBe("adidas-originals");

            await _wordRepoMock.Received(1).UpdateAsync(Arg.Is<Word>(w =>
                w.Name == "Adidas Originals" && w.Categories.Count == 1 && w.Categories[0].CategoryId == "cat-1"
            ), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra nao existir")]
        public async Task ExecuteAsync_WhenWordDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var input = new EditWordInput("id-inexistente", "Novo Nome", null);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Word?)null);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Conflict quando o novo nome ja pertencer a outra palavra")]
        public async Task ExecuteAsync_WhenNewNameBelongsToAnotherWord_ShouldReturnConflict()
        {
            // Arrange
            Word word = WordDataBuilder.Create().WithName("Adidas");
            Word otherWord = WordDataBuilder.Create().WithName("Nike");
            var input = new EditWordInput(word.Id, "Nike", null);

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>()).Returns(new List<Word> { otherWord });

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);

            await _wordRepoMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }
    }
}
