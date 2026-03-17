using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class PurchaseRequestMapper
    {
        public static PurchaseRequest ToPurchaseRequestEntity(this CreatePurchaseRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new PurchaseRequest
            {
                Description = dto.Description,
                Supplier = dto.Supplier,
                RequestedBy = dto.RequestedBy,
                RequestedDeliveryDate = dto.RequestedDeliveryDate,
                IsUrgent = dto.IsUrgent,
                PRItems = dto.PRItems.Select(item => new PurchaseRequestItem
                {
                    LineItemNo = item.LineItemNo,
                    RequestedQuantity = item.RequestedQuantity,
                    Price = item.Price,
                    InventoryItemId = item.InventoryItemId,
                }).ToList(),
            };
        }

        public static PurchaseRequestDto ToPurchaseRequestDto(this PurchaseRequest entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new PurchaseRequestDto
            {
                Id = entity.Id,
                PurchaseRequestCode = entity.PurchaseRequestCode,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                Description = entity.Description,
                Supplier = entity.Supplier,
                RequestedBy = entity.RequestedBy,
                RequestedDeliveryDate = entity.RequestedDeliveryDate,
                IsUrgent = entity.IsUrgent,
                PurchaseRequestStatus = entity.PurchaseRequestStatus,
                ReviewedBy = entity.ReviewedBy,
                ReviewedDate = entity.ReviewedDate,
                PRItems = entity.PRItems.Select(item => new PurchaseRequestItemDto
                {
                    Id = item.Id,
                    LineItemNo = item.LineItemNo,
                    RequestedQuantity = item.RequestedQuantity,
                    Price = item.Price,
                    InventoryItemId = item.InventoryItemId,
                    InventoryItem = item.InventoryItem?.ToInventoryItemDto(),
                }).ToList(),
            };
        }

        public static void ApplyUpdates(this PurchaseRequest entity, UpdatePurchaseRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.Description = dto.Description;
            entity.Supplier = dto.Supplier;
            entity.RequestedBy = dto.RequestedBy;
            entity.RequestedDeliveryDate = dto.RequestedDeliveryDate;
            entity.IsUrgent = dto.IsUrgent;
            entity.PRItems = dto.PRItems.Select(item => new PurchaseRequestItem
            {
                LineItemNo = item.LineItemNo,
                RequestedQuantity = item.RequestedQuantity,
                Price = item.Price,
                InventoryItemId = item.InventoryItemId,
            }).ToList();
        }
    }
}
