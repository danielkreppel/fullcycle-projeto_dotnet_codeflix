namespace FC.Codeflix.Catalog.Application.UseCases.Genre.Common
{
    public class GenreModelOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public IReadOnlyList<Guid> Categories { get; set; }
        public bool IsActive { get; set; }

        public GenreModelOutput(Guid id, string name, DateTime createdAt, IReadOnlyList<Guid> categories, bool isActive)
        {
            Id = id;
            Name = name;
            CreatedAt = createdAt;
            Categories = categories;
            IsActive = isActive;
        }
    }
}
