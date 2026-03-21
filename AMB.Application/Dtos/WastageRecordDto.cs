using System;
using System.Collections.Generic;

namespace AMB.Application.Dtos
{
    public class WastageRecordDto
    {
        public int Id { get; set; }
        public string WastageEntryNumber { get; set; } = string.Empty;
        public DateTimeOffset EntryDate { get; set; }
        public string RecordedBy { get; set; } = string.Empty;
        public string GeneralNotes { get; set; } = string.Empty;
        public List<WastageEntryItemDto> Items { get; set; } = new List<WastageEntryItemDto>();
    }

    public class WastageEntryItemDto
    {
        public int Id { get; set; }
        public int ItemNo { get; set; }
        public string WastageType { get; set; } = string.Empty;
        public float Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int InventoryItemId { get; set; }
        public string InventoryItemName { get; set; } = string.Empty;
    }
}
