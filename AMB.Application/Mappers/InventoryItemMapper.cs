using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class InventoryItemMapper
    {
        public static InventoryItem ToInventoryItemEntity(this CreateInventoryItemRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new InventoryItem
            {
                ItemNumber = dto.ItemNumber,
                ItemName = dto.ItemName,
                OpeningQuantity = dto.OpeningQuantity,
                ItemType = dto.ItemType,
                ItemCategory = dto.ItemCategory,
                UoM = dto.UoM,
                UnitPrice = dto.UnitPrice,
                Currency = dto.Currency,
                Remarks = dto.Remarks,
                MinimumStockLevel = dto.MinimumStockLevel,
                MaximumStockLevel = dto.MaximumStockLevel,
                ReOrderLevel = dto.ReOrderLevel,
                StorageLocation = dto.StorageLocation,
                ShelveLife = dto.ShelveLife,
                StorageConditions = dto.StorageConditions,
                Sku = dto.Sku,
                ExpiryDate = dto.ExpiryDate,
            };
        }

        public static InventoryItemDto ToInventoryItemDto(this InventoryItem entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new InventoryItemDto
            {
                Id = entity.Id,
                ItemNumber = entity.ItemNumber,
                ItemName = entity.ItemName,
                OpeningQuantity = entity.OpeningQuantity,
                RemainingQuantity = entity.RemainingQuantity,
                ItemType = entity.ItemType,
                ItemCategory = entity.ItemCategory,
                UoM = entity.UoM,
                UnitPrice = entity.UnitPrice,
                Currency = entity.Currency,
                Remarks = entity.Remarks,
                MinimumStockLevel = entity.MinimumStockLevel,
                MaximumStockLevel = entity.MaximumStockLevel,
                ReOrderLevel = entity.ReOrderLevel,
                StorageLocation = entity.StorageLocation,
                ShelveLife = entity.ShelveLife,
                StorageConditions = entity.StorageConditions,
                Sku = entity.Sku,
                ExpiryDate = entity.ExpiryDate,
                InventoryStatus = entity.InventoryStatus,
            };
        }

        public static void ApplyUpdates(this InventoryItem entity, UpdateInventoryItemRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.ItemNumber = dto.ItemNumber;
            entity.ItemName = dto.ItemName;
            entity.OpeningQuantity = dto.OpeningQuantity;
            entity.ItemType = dto.ItemType;
            entity.ItemCategory = dto.ItemCategory;
            entity.UoM = dto.UoM;
            entity.UnitPrice = dto.UnitPrice;
            entity.Currency = dto.Currency;
            entity.Remarks = dto.Remarks;
            entity.MinimumStockLevel = dto.MinimumStockLevel;
            entity.MaximumStockLevel = dto.MaximumStockLevel;
            entity.ReOrderLevel = dto.ReOrderLevel;
            entity.StorageLocation = dto.StorageLocation;
            entity.ShelveLife = dto.ShelveLife;
            entity.StorageConditions = dto.StorageConditions;
            entity.Sku = dto.Sku;
            entity.ExpiryDate = dto.ExpiryDate;
        }
    }
}
