using System;
using System.Collections.Generic;

namespace AMB.Application.Dtos
{
    public class CreateWastageRecordDto
    {
        public DateTimeOffset EntryDate { get; set; }
        public string RecordedBy { get; set; } = string.Empty;
        public string GeneralNotes { get; set; } = string.Empty;
        public List<CreateWastageEntryItemDto> Items { get; set; } = new List<CreateWastageEntryItemDto>();
    }

    public class CreateWastageEntryItemDto
    {
        public int ItemNo { get; set; }
        public string WastageType { get; set; } = string.Empty;
        public float Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int InventoryItemId { get; set; }
    }
}
