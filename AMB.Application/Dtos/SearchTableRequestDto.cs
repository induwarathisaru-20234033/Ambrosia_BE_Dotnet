using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class SearchTableRequestDto: BaseSearchRequestDto
    {
        public EntityStatus? Status { get; set; }
    }
}
