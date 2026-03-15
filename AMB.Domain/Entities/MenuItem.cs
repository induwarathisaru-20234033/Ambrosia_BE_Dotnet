namespace AMB.Domain.Entities
{
    public class MenuItem:BaseEntity
    {
        public string Name { get; set; }= string.Empty;

        public decimal Price { get; set; }

        public string Category { get; set; }=string.Empty;

        public bool IsAvailable { get; set; }
    }
}