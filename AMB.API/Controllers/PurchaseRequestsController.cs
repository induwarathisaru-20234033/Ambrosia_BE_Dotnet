using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class PurchaseRequestsController : ControllerBase
    {
        private readonly IPurchaseRequestService _purchaseRequestService;

        public PurchaseRequestsController(IPurchaseRequestService purchaseRequestService)
        {
            _purchaseRequestService = purchaseRequestService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<PurchaseRequestDto>>> Create([FromBody] CreatePurchaseRequestDto request)
        {
            var result = await _purchaseRequestService.CreatePurchaseRequestAsync(request);
            var response = new BaseResponseDto<PurchaseRequestDto>(result, "Purchase request created successfully!");
            return Ok(response);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<BaseResponseDto<PurchaseRequestDto>>> Update(int id, [FromBody] UpdatePurchaseRequestDto request)
        {
            request.Id = id;
            var result = await _purchaseRequestService.UpdatePurchaseRequestAsync(request);
            var response = new BaseResponseDto<PurchaseRequestDto>(result, "Purchase request updated successfully!");
            return Ok(response);
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<ActionResult<BaseResponseDto<PurchaseRequestDto>>> Approve(int id)
        {
            var result = await _purchaseRequestService.ApprovePurchaseRequestAsync(id);
            var response = new BaseResponseDto<PurchaseRequestDto>(result, "Purchase request approved successfully!");
            return Ok(response);
        }

        [HttpPatch("{id:int}/reject")]
        public async Task<ActionResult<BaseResponseDto<PurchaseRequestDto>>> Reject(int id)
        {
            var result = await _purchaseRequestService.RejectPurchaseRequestAsync(id);
            var response = new BaseResponseDto<PurchaseRequestDto>(result, "Purchase request rejected successfully!");
            return Ok(response);
        }
    }
}
