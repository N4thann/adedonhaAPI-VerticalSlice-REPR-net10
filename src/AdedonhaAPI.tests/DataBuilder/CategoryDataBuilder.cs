using AdedonhaAPI.Domain.Entities;
using Bogus;

namespace AdedonhaAPI.tests.DataBuilder
{
    public class CategoryDataBuilder
    {
        private readonly Category _instance;

        public CategoryDataBuilder()
        {
            var faker = new Faker<Category>("pt_BR")
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.Description, f => f.Lorem.Sentence());

            _instance = faker.Generate();
            _instance.Slug = AdedonhaAPI.Domain.Common.SlugGenerator.Generate(_instance.Name);
        }

        public static CategoryDataBuilder Create() => new();
        public Category Build() => _instance;
        public static implicit operator Category(CategoryDataBuilder builder) => builder.Build();

        public CategoryDataBuilder WithId(string id)
        {
            typeof(Category).GetProperty(nameof(Category.Id))?.SetValue(_instance, id);
            return this;
        }

        public CategoryDataBuilder WithName(string name)
        {
            _instance.Name = name;
            _instance.Slug = AdedonhaAPI.Domain.Common.SlugGenerator.Generate(name);
            return this;
        }

        public CategoryDataBuilder WithIsActive(bool isActive)
        {
            _instance.IsActive = isActive;
            return this;
        }

        public static List<Category> AsList(int count)
        {
            var list = new List<Category>();
            for (int i = 0; i < count; i++) list.Add(Create().Build());
            return list;
        }
    }
}
