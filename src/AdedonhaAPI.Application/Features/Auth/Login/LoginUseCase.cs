using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Identity;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Common.Options;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdedonhaAPI.Application.Features.Auth.Login
{
    /// <summary>
    /// Autentica um usuário e emite um token JWT.
    /// </summary>
    public class LoginUseCase : IUseCase<LoginInput, ErrorOr<LoginOutput>>
    {
        private readonly IIdentityService _identityService;
        private readonly IValidator<LoginInput> _validator;
        private readonly JwtOptions _jwtOptions;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<LoginUseCase> _logger;

        public LoginUseCase(
            IIdentityService identityService,
            IValidator<LoginInput> validator,
            IOptions<JwtOptions> jwtOptions,
            IRequestContext requestContext,
            ILogger<LoginUseCase> logger)
        {
            _identityService = identityService;
            _validator = validator;
            _jwtOptions = jwtOptions.Value;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<LoginOutput>> ExecuteAsync(LoginInput input, CancellationToken cancellationToken)
        {
            _logger.LogBegin("Login de usuário", _requestContext);

            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            var user = await _identityService.ValidateCredentialsAsync(input.Email, input.Password, cancellationToken);
            if (user == null)
            {
                _logger.LogEnd("Login de usuário", _requestContext);
                return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "E-mail ou senha incorretos.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = GenerateToken(claims);

            _logger.LogEnd("Login de usuário", _requestContext, new() { ["UserId"] = user.Id });

            return new LoginOutput(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }

        private JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            return new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
        }
    }
}
