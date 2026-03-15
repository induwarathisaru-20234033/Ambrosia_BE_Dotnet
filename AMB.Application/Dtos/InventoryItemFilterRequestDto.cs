using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class InventoryItemFilterRequestDto : BaseSearchRequestDto
    {
        public string? ItemNumber { get; set; }
        public string? ItemName { get; set; }
        public string? ItemType { get; set; }
        public string? ItemCategory { get; set; }
        public string? UoM { get; set; }
        public EntityStatus? Status { get; set; }
        public InventoryStatus? InventoryStatus { get; set; }
    }
}
