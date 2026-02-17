namespace AMB.Application.Dtos
{
    public class PagedResponseDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }

        public List<T> Items { get; set; } = new();
        public int TotalItemCount { get; set; }
    }
}
