using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IGoodsIssueService
    {
        Task<GoodIssueNoteDto> CreateGoodIssueNoteAsync(CreateGoodIssueNoteDto request);
        Task<GoodIssueNoteDto> UpdateGoodIssueNoteAsync(UpdateGoodIssueNoteDto request);
        Task<GoodIssueNoteDto> GetGoodIssueNoteByIdAsync(int id);
        Task<PaginatedResultDto<GoodIssueNoteDto>> GetGoodIssueNotesPagedAsync(GoodIssueNoteFilterRequestDto request);
    }
}
