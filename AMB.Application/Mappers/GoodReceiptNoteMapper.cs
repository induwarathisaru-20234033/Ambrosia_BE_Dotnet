using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class GoodReceiptNoteMapper
    {
        public static GoodReceiptNoteDto ToGoodReceiptNoteDto(this GoodReceiptNote entity)
        {
            return new GoodReceiptNoteDto
            {
                Id = entity.Id,
                GRNNumber = entity.GRNNumber,
                Supplier = entity.Supplier,
                ReceivedDate = entity.ReceivedDate,
                ReceivedBy = entity.ReceivedBy,
                ReceivedFacility = entity.ReceivedFacility,
                PurchaseRequestId = entity.PurchaseRequestId,
                GRNStatus = entity.GRNStatus,
                Items = entity.GRNItems
                    .OrderBy(item => item.LineItemNo)
                    .Select(item => new GRNItemDto
                    {
                        Id = item.Id,
                        LineItemNo = item.LineItemNo,
                        PRItemId = item.PRItemId,
                        PurchaseRequestItem = item.PRItem == null
                            ? null
                            : new PurchaseRequestItemDto
                            {
                                Id = item.PRItem.Id,
                                LineItemNo = item.PRItem.LineItemNo,
                                RequestedQuantity = item.PRItem.RequestedQuantity,
                                Price = item.PRItem.Price,
                                InventoryItemId = item.PRItem.InventoryItemId,
                                InventoryItem = item.PRItem.InventoryItem?.ToInventoryItemDto(),
                            },
                        ReceivedQuantity = item.ReceivedQuantity,
                        AcceptedQuantity = item.AcceptedQuantity,
                        RejectedQuantity = item.RejectedQuantity,
                        TotalPrice = item.TotalPrice,
                    })
                    .ToList(),
            };
        }
    }
}
