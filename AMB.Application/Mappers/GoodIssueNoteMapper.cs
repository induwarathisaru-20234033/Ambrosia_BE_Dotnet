using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class GoodIssueNoteMapper
    {
        public static GoodIssueNoteDto ToGoodIssueNoteDto(this GoodIssueNote entity)
        {
            return new GoodIssueNoteDto
            {
                Id = entity.Id,
                GINumber = entity.GINumber,
                IssuedBy = entity.IssuedBy,
                IssuedDate = entity.IssuedDate,
                Items = entity.GIItems
                    .OrderBy(item => item.LineItemNo)
                    .Select(item => new GIItemDto
                    {
                        Id = item.Id,
                        LineItemNo = item.LineItemNo,
                        InventoryItemId = item.InventoryItemId,
                        InventoryItem = item.InventoryItem?.ToInventoryItemDto(),
                        RequestedQuantity = item.RequestedQuantity,
                        IssuedQuantity = item.IssuedQuantity,
                        IssuedFrom = item.IssuedFrom,
                        IssuedTo = item.IssuedTo,
                        Remarks = item.Remarks,
                    })
                    .ToList(),
            };
        }
    }
}