using AdedonhaAPI.Application.Common.Identity;
using Bogus;

namespace AdedonhaAPI.tests.DataBuilder
{
    public class IdentityUserDataBuilder
    {
        private class UserData
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
        }

        private readonly UserData _instance;

        public IdentityUserDataBuilder()
        {
            var faker = new Faker<UserData>("pt_BR")
                .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Roles, f => new List<string> { IdentityRoles.Admin });

            _instance = faker.Generate();
        }

        public static IdentityUserDataBuilder Create() => new();

        public IdentityUserDto Build() => new(_instance.Id, _instance.Name, _instance.Email, _instance.Roles);

        public static implicit operator IdentityUserDto(IdentityUserDataBuilder builder) => builder.Build();

        public IdentityUserDataBuilder WithEmail(string email)
        {
            _instance.Email = email;
            return this;
        }

        public IdentityUserDataBuilder WithRoles(params string[] roles)
        {
            _instance.Roles = roles.ToList();
            return this;
        }
    }
}
