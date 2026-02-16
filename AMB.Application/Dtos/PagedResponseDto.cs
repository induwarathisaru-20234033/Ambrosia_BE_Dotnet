namespace AMB.Application.Dtos
{
    public class PagedResponseDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
