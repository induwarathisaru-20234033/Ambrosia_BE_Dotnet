using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class SearchOrderRequestDto
    {
        public string? Category { get; set; } // ongoing / completed

        public string? OrderNumber { get; set; }
        public string? TableName { get; set; }
        public string? WaiterName { get; set; }
        public string? CustomerName { get; set; }

        public DateTimeOffset? OrderDateFrom { get; set; }
        public DateTimeOffset? OrderDateTo { get; set; }

        public OrderStatus? Status { get; set; }

        public string? SortField { get; set; }
        public int? SortOrder { get; set; } // 1 = asc, -1 = desc

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}