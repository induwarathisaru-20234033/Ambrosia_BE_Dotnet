using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class CalenderExclusionMapper
    {
        public static CalenderExclusionDto ToCalenderExclusionDto(this CalenderExclusion entity)
        {
            if (entity == null) return null;

            return new CalenderExclusionDto
            {
                Id = entity.Id,
                ExclusionDate = entity.ExclusionDate,
                Reason = entity.Reason
            };
        }
    }
}
