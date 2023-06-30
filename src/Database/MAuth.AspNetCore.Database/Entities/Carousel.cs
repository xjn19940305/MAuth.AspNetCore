namespace MAuth.AspNetCore.Database.Entities
{
    public class Carousel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string FilePath { get; set; }

        public int? Sort { get; set; }
        public string? Link { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateUpdate { get; set; }
    }
}
