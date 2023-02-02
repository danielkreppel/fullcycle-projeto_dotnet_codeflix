using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre
{
    [Collection(nameof(GenreTestFixture))]
    public class GenreTest
    {
        public readonly GenreTestFixture _fixture;

        public GenreTest(GenreTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Genre - Aggregates")]
        public void Instantiate()
        {
            var genreName = _fixture.GetValidName();
            
            var dateTimeBefore = DateTime.Now;

            var genre = new DomainEntity.Genre(genreName);
            
            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            genre.Should().NotBeNull();
            genre.Name.Should().Be(genreName);
            genre.IsActive.Should().BeTrue();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (genre.CreatedAt > dateTimeBefore).Should().BeTrue();
            (genre.CreatedAt < dateTimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstantiateWithActive))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstantiateWithActive(bool isActive)
        {
            var genreName = _fixture.GetValidName();

            var dateTimeBefore = DateTime.Now;

            var genre = new DomainEntity.Genre(genreName, isActive);

            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            genre.Should().NotBeNull();
            genre.Name.Should().Be(genreName);
            genre.IsActive.Should().Be(isActive);
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (genre.CreatedAt > dateTimeBefore).Should().BeTrue();
            (genre.CreatedAt < dateTimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(Activate))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Activate(bool isActive)
        {
            var genre = _fixture.GetExampleGenre(isActive);
            var oldName = genre.Name;

            genre.Activate();

            genre.Should().NotBeNull();
            genre.Name.Should().Be(oldName);
            genre.IsActive.Should().BeTrue();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Theory(DisplayName = nameof(Deactivate))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Deactivate(bool isActive)
        {
            var genre = _fixture.GetExampleGenre(isActive);
            var oldName = genre.Name;

            genre.Deactivate();

            genre.Should().NotBeNull();
            genre.Name.Should().Be(oldName);
            genre.IsActive.Should().BeFalse();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Genre - Aggregates")]
        public void Update()
        {
            var genre = _fixture.GetExampleGenre();
            var newName = _fixture.GetValidName();
            var oldIsActive = genre.IsActive;

            genre.Update(newName);

            genre.Should().NotBeNull();
            genre.Name.Should().Be(newName);
            genre.IsActive.Should().Be(oldIsActive);
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Theory(DisplayName = nameof(InstantiateThrowWhenNameEmpty))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void InstantiateThrowWhenNameEmpty(string? name)
        {
            var action = () => new DomainEntity.Genre(name);

            action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null");
        }

        [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateThrowWhenNameIsEmpty(string? name)
        {
            var genre = _fixture.GetExampleGenre();

            var action = () => genre.Update(name);

            action.Should().Throw<EntityValidationException>().WithMessage("Name should not be empty or null");
        }
    }
}
