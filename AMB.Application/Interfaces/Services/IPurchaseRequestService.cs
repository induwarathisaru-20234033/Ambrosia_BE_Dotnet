using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IPurchaseRequestService
    {
        Task<PurchaseRequestDto> CreatePurchaseRequestAsync(CreatePurchaseRequestDto request);
        Task<PurchaseRequestDto> UpdatePurchaseRequestAsync(UpdatePurchaseRequestDto request);
        Task<PurchaseRequestDto> GetPurchaseRequestByIdAsync(int id);
        Task<PaginatedResultDto<PurchaseRequestDto>> GetPurchaseRequestsPagedAsync(PurchaseRequestFilterRequestDto request);
        Task<PurchaseRequestDto> ApprovePurchaseRequestAsync(int id);
        Task<PurchaseRequestDto> RejectPurchaseRequestAsync(int id);
    }
}
