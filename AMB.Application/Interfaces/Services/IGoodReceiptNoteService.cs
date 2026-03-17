using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IGoodReceiptNoteService
    {
        Task<GoodReceiptNoteDto> CreateGoodReceiptNoteAsync(CreateGoodReceiptNoteDto request);
        Task<PaginatedResultDto<GoodReceiptNoteDto>> GetGoodReceiptNotesPagedAsync(GoodReceiptNoteFilterRequestDto request);
    }
}
