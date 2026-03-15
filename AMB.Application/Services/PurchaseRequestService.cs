using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;

namespace AMB.Application.Services
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IPurchaseRequestRepositoy _purchaseRequestRepositoy;

        public PurchaseRequestService(IPurchaseRequestRepositoy purchaseRequestRepositoy)
        {
            _purchaseRequestRepositoy = purchaseRequestRepositoy;
        }

        public async Task<PurchaseRequestDto> CreatePurchaseRequestAsync(CreatePurchaseRequestDto request)
        {
            var purchaseRequest = request.ToPurchaseRequestEntity();
            purchaseRequest.PurchaseRequestCode = await GenerateUniquePurchaseRequestCodeAsync();
            purchaseRequest.Status = (int)EntityStatus.Active;
            purchaseRequest.PurchaseRequestStatus = (int)PurchaseRequestStatus.ApprovalPending;

            foreach (var item in purchaseRequest.PRItems)
            {
                item.Status = (int)EntityStatus.Active;
            }

            var createdPurchaseRequest = await _purchaseRequestRepositoy.AddAsync(purchaseRequest);
            return createdPurchaseRequest.ToPurchaseRequestDto();
        }

        public async Task<PurchaseRequestDto> UpdatePurchaseRequestAsync(UpdatePurchaseRequestDto request)
        {
            var existing = await _purchaseRequestRepositoy.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Purchase request with ID {request.Id} not found.");
            }

            existing.ApplyUpdates(request);
            existing.PurchaseRequestStatus = (int)PurchaseRequestStatus.ApprovalPending;
            existing.Status = (int)EntityStatus.Active;

            foreach (var item in existing.PRItems)
            {
                item.Status = (int)EntityStatus.Active;
            }

            var updated = await _purchaseRequestRepositoy.UpdateAsync(existing);
            if (updated == null)
            {
                throw new KeyNotFoundException($"Purchase request with ID {request.Id} not found.");
            }

            return updated.ToPurchaseRequestDto();
        }

        public async Task<PurchaseRequestDto> ApprovePurchaseRequestAsync(int id)
        {
            return await ReviewPurchaseRequestAsync(id, PurchaseRequestStatus.Approved);
        }

        public async Task<PurchaseRequestDto> RejectPurchaseRequestAsync(int id)
        {
            return await ReviewPurchaseRequestAsync(id, PurchaseRequestStatus.Rejected);
        }

        private async Task<string> GenerateUniquePurchaseRequestCodeAsync()
        {
            const int maxAttempts = 10;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
                var code = $"PR-{datePart}-{randomPart}";

                var exists = await _purchaseRequestRepositoy.ExistsByCodeAsync(code);
                if (!exists)
                {
                    return code;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique purchase request code.");
        }

        private async Task<PurchaseRequestDto> ReviewPurchaseRequestAsync(int id, PurchaseRequestStatus reviewStatus)
        {
            var existing = await _purchaseRequestRepositoy.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Purchase request with ID {id} not found.");
            }

            if (existing.PurchaseRequestStatus != (int)PurchaseRequestStatus.ApprovalPending)
            {
                throw new InvalidOperationException("Only purchase requests with Approval Pending status can be reviewed.");
            }

            existing.PurchaseRequestStatus = (int)reviewStatus;

            var updated = await _purchaseRequestRepositoy.UpdateAsync(existing);
            if (updated == null)
            {
                throw new KeyNotFoundException($"Purchase request with ID {id} not found.");
            }

            return updated.ToPurchaseRequestDto();
        }
    }
}
