using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IGoodReceiptNoteService
    {
        Task<GoodReceiptNoteDto> CreateGoodReceiptNoteAsync(CreateGoodReceiptNoteDto request);
    }
}
