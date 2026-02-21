using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class CalenderExclusionMapper
    {
        public static CalenderExclusion ToCalenderExclusionEntity(this CreateCalenderExclusionRequestDto dto)
        {
            if (dto == null) return null;

            return new CalenderExclusion
            {
                ExclusionDate = dto.ExclusionDate,
                Reason = dto.Reason
            };
        }

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
