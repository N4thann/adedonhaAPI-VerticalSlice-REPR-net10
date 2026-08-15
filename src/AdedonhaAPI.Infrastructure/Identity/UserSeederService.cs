using AdedonhaAPI.Application.Common.Identity;
using AdedonhaAPI.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdedonhaAPI.Infrastructure.Identity
{
    /// <summary>
    /// Garante as roles Admin/User e cria o usuario administrador inicial na primeira subida
    /// (coleção de usuários vazia). Idempotente — não faz nada em subidas seguintes.
    /// </summary>
    public class UserSeederService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserSeederService> _logger;

        public UserSeederService(IServiceProvider serviceProvider, ILogger<UserSeederService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminUserSeedOptions>>().Value;

            await EnsureRolesExistAsync(roleManager);

            if (userManager.Users.Any())
            {
                _logger.LogInformation("Usuário administrador já existe, seed ignorado.");
                return;
            }

            _logger.LogInformation("Iniciando seed do usuário administrador...");

            var adminUser = new ApplicationUser
            {
                UserName = options.Name,
                Email = options.Email,
                Name = options.Name,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, options.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Falha ao criar o usuário administrador inicial: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            var roleResult = await userManager.AddToRoleAsync(adminUser, IdentityRoles.Admin);
            if (!roleResult.Succeeded)
                throw new InvalidOperationException($"Falha ao associar o usuário administrador inicial à role {IdentityRoles.Admin}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

            _logger.LogInformation("Usuário administrador criado: {Email}", options.Email);
        }

        private static async Task EnsureRolesExistAsync(RoleManager<ApplicationRole> roleManager)
        {
            foreach (var roleName in new[] { IdentityRoles.Admin, IdentityRoles.User })
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
