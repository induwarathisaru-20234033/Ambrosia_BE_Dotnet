using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMB.Application.Services
{
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IPurchaseRequestRepositoy _purchaseRequestRepositoy;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseRequestService(
            IPurchaseRequestRepositoy purchaseRequestRepositoy,
            IEmployeeRepository employeeRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _purchaseRequestRepositoy = purchaseRequestRepositoy;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<PurchaseRequestDto> GetPurchaseRequestByIdAsync(int id)
        {
            var purchaseRequest = await _purchaseRequestRepositoy.GetByIdAsync(id);
            if (purchaseRequest == null)
            {
                throw new KeyNotFoundException($"Purchase request with ID {id} not found.");
            }

            var dto = purchaseRequest.ToPurchaseRequestDto();

            if (!string.IsNullOrWhiteSpace(purchaseRequest.CreatedBy))
            {
                var creatorNames = await _purchaseRequestRepositoy
                    .GetCreatorNamesByUsernamesAsync(new List<string> { purchaseRequest.CreatedBy });

                if (creatorNames.TryGetValue(purchaseRequest.CreatedBy, out var creatorName)
                    && !string.IsNullOrWhiteSpace(creatorName))
                {
                    dto.CreatedBy = creatorName;
                }
            }

            return dto;
        }

        public async Task<PaginatedResultDto<PurchaseRequestDto>> GetPurchaseRequestsPagedAsync(PurchaseRequestFilterRequestDto request)
        {
            var query = _purchaseRequestRepositoy.GetSearchQuery();

            if (!string.IsNullOrWhiteSpace(request.PurchaseRequestCode))
            {
                query = query.Where(pr => pr.PurchaseRequestCode.Contains(request.PurchaseRequestCode));
            }

            if (!string.IsNullOrWhiteSpace(request.Supplier))
            {
                query = query.Where(pr => pr.Supplier.Contains(request.Supplier));
            }

            if (!string.IsNullOrWhiteSpace(request.RequestedBy))
            {
                query = query.Where(pr => pr.RequestedBy.Contains(request.RequestedBy));
            }

            if (request.PurchaseRequestStatus.HasValue)
            {
                query = query.Where(pr => pr.PurchaseRequestStatus == request.PurchaseRequestStatus.Value);
            }

            if (request.CreatedDateFrom.HasValue)
            {
                query = query.Where(pr => pr.CreatedDate >= request.CreatedDateFrom.Value);
            }

            if (request.CreatedDateTo.HasValue)
            {
                query = query.Where(pr => pr.CreatedDate <= request.CreatedDateTo.Value);
            }

            var totalItemCount = await query.CountAsync();

            var pageCount = request.PageSize == 0
                ? 0
                : (int)Math.Ceiling(totalItemCount / (double)request.PageSize);

            if (request.PageSize > 0)
            {
                query = query
                    .OrderByDescending(pr => pr.CreatedDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var purchaseRequests = await query.ToListAsync();

            var creatorUsernames = purchaseRequests
                .Select(pr => pr.CreatedBy)
                .Where(createdBy => !string.IsNullOrWhiteSpace(createdBy))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var creatorNames = await _purchaseRequestRepositoy.GetCreatorNamesByUsernamesAsync(creatorUsernames);

            var items = purchaseRequests
                .Select(pr =>
                {
                    var dto = pr.ToPurchaseRequestDto();
                    if (!string.IsNullOrWhiteSpace(pr.CreatedBy)
                        && creatorNames.TryGetValue(pr.CreatedBy, out var creatorName)
                        && !string.IsNullOrWhiteSpace(creatorName))
                    {
                        dto.CreatedBy = creatorName;
                    }

                    return dto;
                })
                .ToList();

            return new PaginatedResultDto<PurchaseRequestDto>
            {
                PageCount = pageCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                TotalItemCount = totalItemCount,
                Items = items,
            };
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
            existing.ReviewedBy = await GetCurrentReviewerAsync();
            existing.ReviewedDate = DateTimeOffset.UtcNow;

            var updated = await _purchaseRequestRepositoy.UpdateAsync(existing);
            if (updated == null)
            {
                throw new KeyNotFoundException($"Purchase request with ID {id} not found.");
            }

            return updated.ToPurchaseRequestDto();
        }

        private async Task<string> GetCurrentReviewerAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            var auth0UserId = context?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(auth0UserId))
            {
                return "SYSTEM";
            }

            var employee = await _employeeRepository.GetByUserIDAsync(auth0UserId);
            return !string.IsNullOrWhiteSpace(employee?.FirstName)
                ? $"{employee!.FirstName} {employee.LastName}".Trim()
                : "SYSTEM";
        }
    }
}
