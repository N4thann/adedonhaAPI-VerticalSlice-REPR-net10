using AdedonhaAPI.Shared.Identity;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AdedonhaAPI.Features.Admin.Users.CreateAdmin
{
    public class SeedAdminUserHandler : ICommandHandler<SeedAdminUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        // Injetamos os serviços do Identity (que já configuramos!)
        public SeedAdminUserHandler(UserManager<ApplicationUser> userManager,
                                    RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> Handle(SeedAdminUserCommand request, CancellationToken ct)
        {
            // 1. Criar a Role "Admin" (se não existir)
            var roleExists = await _roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new ApplicationRole("Admin"));
            }

            // 2. Criar o Usuário "admin" (se não existir)
            var user = await _userManager.FindByNameAsync("admin");
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@email.com",
                    EmailConfirmed = true, // Confirma o email automaticamente
                    Id = Guid.NewGuid()
                };

                // Use uma senha forte!
                var result = await _userManager.CreateAsync(user, "SuaSenhaForte@123");
                if (!result.Succeeded)
                {
                    return "Falha ao criar usuário: " + result.Errors.First().Description;
                }

                // 3. Adicionar o usuário à role "Admin"
                await _userManager.AddToRoleAsync(user, "Admin");

                return "Usuário Admin criado com sucesso!";
            }

            return "Usuário Admin já existe.";
        }
    }
}
