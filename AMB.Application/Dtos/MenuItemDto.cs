namespace AMB.Application.Dtos
{
    public class MenuItemDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public bool IsAvailable { get; set; }
    }
}