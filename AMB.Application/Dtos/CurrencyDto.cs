namespace AMB.Application.Dtos
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}