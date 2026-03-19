using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IGoodReceiptNoteService
    {
        Task<GoodReceiptNoteDto> CreateGoodReceiptNoteAsync(CreateGoodReceiptNoteDto request);
        Task<GoodReceiptNoteDto> GetGoodReceiptNoteByIdAsync(int id);
        Task<GoodReceiptNoteDto> UpdateGoodReceiptNoteAsync(UpdateGoodReceiptNoteDto request);
        Task<PaginatedResultDto<GoodReceiptNoteDto>> GetGoodReceiptNotesPagedAsync(GoodReceiptNoteFilterRequestDto request);
    }
}
