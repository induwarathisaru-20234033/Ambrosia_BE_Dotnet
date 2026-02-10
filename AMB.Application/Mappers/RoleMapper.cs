using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class RoleMapper
    {
        public static Role ToRoleEntity(this CreateRoleRequestDto dto)
        {
            if (dto == null) return null;
            return new Role
            {
                RoleCode = dto.RoleCode,
                RoleName = dto.Name,
                Description = dto.Description,
                Status = dto.Status == "ENABLED" ? 1 : 0
            };
        }

        public static RoleDto ToRoleDto(this Role entity)
        {
            if (entity == null) return null;

            return new RoleDto
            {
                RoleCode = entity.RoleCode,
                Name = entity.RoleName,
                Description = entity.Description,
                Status = entity.Status == 1 ? "ENABLED" : "DISABLE"
            };
        }

        public static PermissionItemDto ToPermissionItemDto(this Permission entity)
        {
            if (entity == null) return null;

            return new PermissionItemDto
            {
                PermissionCode = entity.PermissionCode,
                Name = entity.PermissionName
            };
        }
    }
}
