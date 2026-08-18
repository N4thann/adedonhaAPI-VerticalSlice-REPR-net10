using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Identity;
using AdedonhaAPI.Application.Common.Options;
using AdedonhaAPI.Application.Features.Auth.Login;
using AdedonhaAPI.tests.DataBuilder;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AdedonhaAPI.tests.UseCases.Auth
{
    public class LoginUseCaseTests
    {
        private readonly IIdentityService _identityServiceMock;
        private readonly IValidator<LoginInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<LoginUseCase> _loggerMock;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly LoginUseCase _sut;

        public LoginUseCaseTests()
        {
            _identityServiceMock = Substitute.For<IIdentityService>();
            _validatorMock = Substitute.For<IValidator<LoginInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<LoginUseCase>>();
            _jwtOptions = Options.Create(new JwtOptions
            {
                ValidAudience = "adedonha-tests",
                ValidIssuer = "adedonha-tests",
                SecretKey = "chave-secreta-de-teste-com-mais-de-32-caracteres",
                TokenValidityInMinutes = 60,
                RefreshTokenValidInMinutes = 120
            });

            _sut = new LoginUseCase(_identityServiceMock, _validatorMock, _jwtOptions, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve emitir um token JWT valido quando as credenciais forem corretas")]
        public async Task ExecuteAsync_WhenCredentialsAreValid_ShouldReturnToken()
        {
            // Arrange
            var input = new LoginInput("admin@adedonha.com", "SenhaForte123!");
            IdentityUserDto user = IdentityUserDataBuilder.Create().WithEmail("admin@adedonha.com").WithRoles(IdentityRoles.Admin);

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _identityServiceMock.ValidateCredentialsAsync(input.Email, input.Password, Arg.Any<CancellationToken>()).Returns(user);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Token.ShouldNotBeNullOrEmpty();
            result.Value.Expiration.ShouldBeGreaterThan(DateTime.UtcNow);

            var decoded = new JwtSecurityTokenHandler().ReadJwtToken(result.Value.Token);
            decoded.Claims.ShouldContain(c => c.Type == ClaimTypes.Email && c.Value == "admin@adedonha.com");
            decoded.Claims.ShouldContain(c => c.Type == ClaimTypes.Role && c.Value == IdentityRoles.Admin);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Validation quando o e-mail ou a senha estiverem vazios")]
        public async Task ExecuteAsync_WhenInputIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var input = new LoginInput("", "");
            var failures = new List<ValidationFailure> { new("Email", "O e-mail é obrigatório.") };
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult(failures));

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Validation);

            await _identityServiceMock.DidNotReceiveWithAnyArgs().ValidateCredentialsAsync(default!, default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Unauthorized quando as credenciais forem invalidas")]
        public async Task ExecuteAsync_WhenCredentialsAreInvalid_ShouldReturnUnauthorized()
        {
            // Arrange
            var input = new LoginInput("admin@adedonha.com", "senha-errada");
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _identityServiceMock.ValidateCredentialsAsync(input.Email, input.Password, Arg.Any<CancellationToken>()).Returns((IdentityUserDto?)null);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Unauthorized);
            result.FirstError.Code.ShouldBe("Auth.InvalidCredentials");
        }
    }
}
