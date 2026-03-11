namespace AMB.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public bool IsAvailable { get; set; }
    }
}