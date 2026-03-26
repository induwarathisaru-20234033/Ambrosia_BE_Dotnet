using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AMB.Application.Services
{
    public class GoodReceiptNoteService : IGoodReceiptNoteService
    {
        private readonly IGoodReceiptNoteRepository _goodReceiptNoteRepository;

        public GoodReceiptNoteService(IGoodReceiptNoteRepository goodReceiptNoteRepository)
        {
            _goodReceiptNoteRepository = goodReceiptNoteRepository;
        }

        public async Task<GoodReceiptNoteDto> CreateGoodReceiptNoteAsync(CreateGoodReceiptNoteDto request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("At least one GRN item is required.");
            }

            var prItemIds = request.Items
                .Select(item => item.PRItemId)
                .Distinct()
                .ToList();

            var prItems = await _goodReceiptNoteRepository.GetPurchaseRequestItemsByIdsAsync(prItemIds);

            var missingPrItemIds = prItemIds
                .Except(prItems.Select(item => item.Id))
                .ToList();

            if (missingPrItemIds.Count > 0)
            {
                throw new ArgumentException($"Invalid PR item IDs: {string.Join(", ", missingPrItemIds)}.");
            }

            var purchaseRequestIds = prItems
                .Select(item => item.PurchaseRequestId)
                .Distinct()
                .ToList();

            if (purchaseRequestIds.Count != 1)
            {
                throw new ArgumentException("All GRN items must belong to the same purchase request.");
            }

            var purchaseRequestId = purchaseRequestIds[0];
            var supplier = prItems.First().PurchaseRequest?.Supplier ?? string.Empty;
            var priceByPrItemId = prItems.ToDictionary(item => item.Id, item => item.Price);

            var goodReceiptNote = new GoodReceiptNote
            {
                GRNNumber = await GenerateUniqueGrnNumberAsync(),
                Supplier = supplier,
                ReceivedBy = request.ReceivedBy,
                ReceivedDate = request.ReceivedDate,
                ReceivedFacility = request.ReceivingFacility,
                PurchaseRequestId = purchaseRequestId,
                GRNStatus = request.GRNStatus,
                Status = (int)EntityStatus.Active,
                GRNItems = request.Items.Select((item, index) => new GRNItem
                {
                    LineItemNo = item.LineItemNo,
                    PRItemId = item.PRItemId,
                    Remarks = item.Remarks ?? string.Empty,
                    ReceivedQuantity = item.ReceivedQuantity,
                    AcceptedQuantity = item.AcceptedQuantity,
                    RejectedQuantity = item.RejectedQuantity,
                    TotalPrice = item.TotalPrice > 0 ? item.TotalPrice : priceByPrItemId[item.PRItemId] * (decimal)item.ReceivedQuantity,
                    Status = (int)EntityStatus.Active,
                }).ToList(),
            };

            var createdGrn = await _goodReceiptNoteRepository.AddAsync(goodReceiptNote);

            if (createdGrn.GRNStatus == (int)GRNStatus.Posted)
            {
                await _goodReceiptNoteRepository.ProcessPostedGrnAsync(createdGrn.Id);
                createdGrn = await _goodReceiptNoteRepository.GetByIdAsync(createdGrn.Id) ?? createdGrn;
            }

            return createdGrn.ToGoodReceiptNoteDto();
        }

        public async Task<GoodReceiptNoteDto> UpdateGoodReceiptNoteAsync(UpdateGoodReceiptNoteDto request)
        {
            var existing = await _goodReceiptNoteRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"GRN with ID {request.Id} not found.");
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("At least one GRN item is required.");
            }

            var prItemIds = request.Items
                .Select(item => item.PRItemId)
                .Distinct()
                .ToList();

            var prItems = await _goodReceiptNoteRepository.GetPurchaseRequestItemsByIdsAsync(prItemIds);

            var missingPrItemIds = prItemIds
                .Except(prItems.Select(item => item.Id))
                .ToList();

            if (missingPrItemIds.Count > 0)
            {
                throw new ArgumentException($"Invalid PR item IDs: {string.Join(", ", missingPrItemIds)}.");
            }

            var purchaseRequestIds = prItems
                .Select(item => item.PurchaseRequestId)
                .Distinct()
                .ToList();

            if (purchaseRequestIds.Count != 1)
            {
                throw new ArgumentException("All GRN items must belong to the same purchase request.");
            }

            var purchaseRequestId = purchaseRequestIds[0];
            var supplier = prItems.First().PurchaseRequest?.Supplier ?? string.Empty;
            var priceByPrItemId = prItems.ToDictionary(item => item.Id, item => item.Price);

            existing.Supplier = supplier;
            existing.ReceivedBy = request.ReceivedBy;
            existing.ReceivedDate = request.ReceivedDate;
            existing.ReceivedFacility = request.ReceivingFacility;
            existing.PurchaseRequestId = purchaseRequestId;
            var wasPosted = existing.GRNStatus == (int)GRNStatus.Posted;
            existing.GRNStatus = request.GRNStatus;
            existing.Status = (int)EntityStatus.Active;
            existing.GRNItems = request.Items.Select(item => new GRNItem
            {
                LineItemNo = item.LineItemNo,
                PRItemId = item.PRItemId,
                Remarks = item.Remarks ?? string.Empty,
                ReceivedQuantity = item.ReceivedQuantity,
                AcceptedQuantity = item.AcceptedQuantity,
                RejectedQuantity = item.RejectedQuantity,
                TotalPrice = item.TotalPrice > 0 ? item.TotalPrice : priceByPrItemId[item.PRItemId] * (decimal)item.ReceivedQuantity,
                Status = (int)EntityStatus.Active,
            }).ToList();

            var updated = await _goodReceiptNoteRepository.UpdateAsync(existing);
            if (updated == null)
            {
                throw new KeyNotFoundException($"GRN with ID {request.Id} not found.");
            }

            if (!wasPosted && updated.GRNStatus == (int)GRNStatus.Posted)
            {
                await _goodReceiptNoteRepository.ProcessPostedGrnAsync(updated.Id);
                updated = await _goodReceiptNoteRepository.GetByIdAsync(updated.Id) ?? updated;
            }

            return updated.ToGoodReceiptNoteDto();
        }

        public async Task<GoodReceiptNoteDto> GetGoodReceiptNoteByIdAsync(int id)
        {
            var grn = await _goodReceiptNoteRepository.GetByIdAsync(id);
            if (grn == null)
            {
                throw new KeyNotFoundException($"GRN with ID {id} not found.");
            }

            return grn.ToGoodReceiptNoteDto();
        }

        public async Task<PaginatedResultDto<GoodReceiptNoteDto>> GetGoodReceiptNotesPagedAsync(GoodReceiptNoteFilterRequestDto request)
        {
            var query = _goodReceiptNoteRepository.GetSearchQuery();

            if (!string.IsNullOrWhiteSpace(request.GRNNumber))
            {
                query = query.Where(grn => grn.GRNNumber.Contains(request.GRNNumber));
            }

            if (!string.IsNullOrWhiteSpace(request.Supplier))
            {
                query = query.Where(grn => grn.Supplier.Contains(request.Supplier));
            }

            if (!string.IsNullOrWhiteSpace(request.ReceivedBy))
            {
                query = query.Where(grn => grn.ReceivedBy.Contains(request.ReceivedBy));
            }

            if (request.GRNStatus.HasValue)
            {
                query = query.Where(grn => grn.GRNStatus == request.GRNStatus.Value);
            }

            if (request.ReceivedDateFrom.HasValue)
            {
                query = query.Where(grn => grn.ReceivedDate >= request.ReceivedDateFrom.Value);
            }

            if (request.ReceivedDateTo.HasValue)
            {
                query = query.Where(grn => grn.ReceivedDate <= request.ReceivedDateTo.Value);
            }

            var totalItemCount = await query.CountAsync();

            var pageCount = request.PageSize == 0
                ? 0
                : (int)Math.Ceiling(totalItemCount / (double)request.PageSize);

            if (request.PageSize > 0)
            {
                query = query
                    .OrderByDescending(grn => grn.CreatedDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var grns = await query.ToListAsync();

            return new PaginatedResultDto<GoodReceiptNoteDto>
            {
                PageCount = pageCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                TotalItemCount = totalItemCount,
                Items = grns.Select(grn => grn.ToGoodReceiptNoteDto()).ToList(),
            };
        }

        private async Task<string> GenerateUniqueGrnNumberAsync()
        {
            const int maxAttempts = 10;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
                var code = $"GRN-{datePart}-{randomPart}";

                var exists = await _goodReceiptNoteRepository.ExistsByNumberAsync(code);
                if (!exists)
                {
                    return code;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique GRN number.");
        }
    }
}
