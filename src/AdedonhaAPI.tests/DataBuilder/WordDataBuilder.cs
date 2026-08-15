using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using Bogus;

namespace AdedonhaAPI.tests.DataBuilder
{
    public class WordDataBuilder
    {
        private readonly Word _instance;

        public WordDataBuilder()
        {
            var faker = new Faker<Word>("pt_BR")
                .RuleFor(w => w.Name, f => f.Commerce.Product())
                .RuleFor(w => w.Description, f => f.Lorem.Sentence());

            _instance = faker.Generate();
            _instance.Slug = SlugGenerator.Generate(_instance.Name);
            _instance.InitialLetter = SlugGenerator.GetInitialLetter(_instance.Name);
        }

        public static WordDataBuilder Create() => new();
        public Word Build() => _instance;
        public static implicit operator Word(WordDataBuilder builder) => builder.Build();

        public WordDataBuilder WithId(string id)
        {
            typeof(Word).GetProperty(nameof(Word.Id))?.SetValue(_instance, id);
            return this;
        }

        public WordDataBuilder WithName(string name)
        {
            _instance.Name = name;
            _instance.Slug = SlugGenerator.Generate(name);
            _instance.InitialLetter = SlugGenerator.GetInitialLetter(name);
            return this;
        }

        public WordDataBuilder WithCategories(List<Word.CategoryInfo> categories)
        {
            _instance.Categories = categories;
            return this;
        }

        public WordDataBuilder WithIsActive(bool isActive)
        {
            _instance.IsActive = isActive;
            return this;
        }

        public static List<Word> AsList(int count)
        {
            var list = new List<Word>();
            for (int i = 0; i < count; i++) list.Add(Create().Build());
            return list;
        }
    }
}
