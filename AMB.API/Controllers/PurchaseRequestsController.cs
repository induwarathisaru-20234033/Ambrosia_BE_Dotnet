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

        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PaginatedResultDto<PurchaseRequestDto>>>> GetPaginated([FromQuery] PurchaseRequestFilterRequestDto request)
        {
            if (request.PageNumber < 1)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<PurchaseRequestDto>>(
                    "Invalid pagination parameters.",
                    new List<string> { "pageNumber must be greater than zero." });

                return BadRequest(errorResponse);
            }

            if (request.CreatedDateFrom.HasValue
                && request.CreatedDateTo.HasValue
                && request.CreatedDateFrom.Value > request.CreatedDateTo.Value)
            {
                var errorResponse = new BaseResponseDto<PaginatedResultDto<PurchaseRequestDto>>(
                    "Invalid date range.",
                    new List<string> { "createdDateFrom must be less than or equal to createdDateTo." });

                return BadRequest(errorResponse);
            }

            var result = await _purchaseRequestService.GetPurchaseRequestsPagedAsync(request);
            var response = new BaseResponseDto<PaginatedResultDto<PurchaseRequestDto>>(
                result,
                "Purchase requests retrieved successfully!");

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BaseResponseDto<PurchaseRequestDto>>> GetById(int id)
        {
            if (id <= 0)
            {
                var errorResponse = new BaseResponseDto<PurchaseRequestDto>(
                    "Invalid id.",
                    new List<string> { "id must be greater than zero." });

                return BadRequest(errorResponse);
            }

            var result = await _purchaseRequestService.GetPurchaseRequestByIdAsync(id);
            var response = new BaseResponseDto<PurchaseRequestDto>(result, "Purchase request retrieved successfully!");

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
