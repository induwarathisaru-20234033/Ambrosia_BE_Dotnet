using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Domain.Enums;

namespace AMB.Application.Services
{
    public class GoodsIssueService : IGoodsIssueService
    {
        private readonly IGoodsIssueRepository _goodsIssueRepository;

        public GoodsIssueService(IGoodsIssueRepository goodsIssueRepository)
        {
            _goodsIssueRepository = goodsIssueRepository;
        }

        public async Task<GoodIssueNoteDto> CreateGoodIssueNoteAsync(CreateGoodIssueNoteDto request)
        {
            await ValidateCreateOrUpdateRequestAsync(request.Items, null);

            var goodIssueNote = new GoodIssueNote
            {
                GINumber = await GenerateUniqueGiNumberAsync(),
                IssuedBy = request.IssuedBy,
                IssuedDate = request.IssuedDate,
                Status = (int)EntityStatus.Active,
                GIItems = request.Items.Select(item => new GIItem
                {
                    LineItemNo = item.LineItemNo,
                    InventoryItemId = item.InventoryItemId,
                    RequestedQuantity = item.RequestedQuantity,
                    IssuedQuantity = item.IssuedQuantity,
                    IssuedFrom = item.IssuedFrom,
                    IssuedTo = item.IssuedTo,
                    Remarks = item.Remarks ?? string.Empty,
                    Status = (int)EntityStatus.Active,
                }).ToList(),
            };

            var created = await _goodsIssueRepository.AddAsync(goodIssueNote);

            return created.ToGoodIssueNoteDto();
        }

        public async Task<GoodIssueNoteDto> UpdateGoodIssueNoteAsync(UpdateGoodIssueNoteDto request)
        {
            var existing = await _goodsIssueRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"GI note with ID {request.Id} not found.");
            }

            await ValidateCreateOrUpdateRequestAsync(request.Items, existing);

            existing.IssuedBy = request.IssuedBy;
            existing.IssuedDate = request.IssuedDate;
            existing.Status = (int)EntityStatus.Active;
            existing.GIItems = request.Items.Select(item => new GIItem
            {
                LineItemNo = item.LineItemNo,
                InventoryItemId = item.InventoryItemId,
                RequestedQuantity = item.RequestedQuantity,
                IssuedQuantity = item.IssuedQuantity,
                IssuedFrom = item.IssuedFrom,
                IssuedTo = item.IssuedTo,
                Remarks = item.Remarks ?? string.Empty,
                Status = (int)EntityStatus.Active,
            }).ToList();

            var updated = await _goodsIssueRepository.UpdateAsync(existing);
            if (updated == null)
            {
                throw new KeyNotFoundException($"GI note with ID {request.Id} not found.");
            }

            return updated.ToGoodIssueNoteDto();
        }

        private async Task<List<InventoryItem>> ValidateCreateOrUpdateRequestAsync(List<CreateGIItemDto> items, GoodIssueNote? existingNote)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("At least one GI item is required.");
            }

            var inventoryItemIds = items
                .Select(item => item.InventoryItemId)
                .Distinct()
                .ToList();

            var inventoryItems = await _goodsIssueRepository.GetInventoryItemsByIdsAsync(inventoryItemIds);

            var missingInventoryItemIds = inventoryItemIds
                .Except(inventoryItems.Select(item => item.Id))
                .ToList();

            if (missingInventoryItemIds.Count > 0)
            {
                throw new ArgumentException($"Invalid inventory item IDs: {string.Join(", ", missingInventoryItemIds)}.");
            }

            var currentIssuedByInventoryId = existingNote == null
                ? new Dictionary<int, float>()
                : existingNote.GIItems
                    .GroupBy(item => item.InventoryItemId)
                    .ToDictionary(group => group.Key, group => group.Sum(item => item.IssuedQuantity));

            var availableByInventoryId = inventoryItems.ToDictionary(
                item => item.Id,
                item => item.RemainingQuantity + (currentIssuedByInventoryId.TryGetValue(item.Id, out var currentIssued) ? currentIssued : 0));

            var totalIssuedByInventoryId = items
                .GroupBy(item => item.InventoryItemId)
                .ToDictionary(group => group.Key, group => group.Sum(item => item.IssuedQuantity));

            var insufficientStockErrors = totalIssuedByInventoryId
                .Where(item => item.Value > availableByInventoryId[item.Key])
                .Select(item => $"Inventory item ID {item.Key} has insufficient stock. Requested to issue {item.Value}, available {availableByInventoryId[item.Key]}.")
                .ToList();

            if (insufficientStockErrors.Count > 0)
            {
                throw new ArgumentException(string.Join(" ", insufficientStockErrors));
            }

            return inventoryItems;
        }

        private async Task<string> GenerateUniqueGiNumberAsync()
        {
            const int maxAttempts = 10;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
                var code = $"GIN-{datePart}-{randomPart}";

                var exists = await _goodsIssueRepository.ExistsByNumberAsync(code);
                if (!exists)
                {
                    return code;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique GI number.");
        }
    }
}