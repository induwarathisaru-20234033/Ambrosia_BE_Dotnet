using AMB.Application.Dtos;
using AMB.Domain.Entities;
using AMB.Domain.Enums;

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
                Description = dto.Description
            };
        }

        public static RoleDto ToRoleDto(this Role entity)
        {
            if (entity == null) return null;

            return new RoleDto
            {
                Id = entity.Id,
                RoleCode = entity.RoleCode,
                Name = entity.RoleName,
                Description = entity.Description,
                Status = entity.Status,
                Permissions = new List<PermissionDto>()
            };
        }

        public static PermissionItemDto ToPermissionItemDto(this Permission entity)
        {
            if (entity == null) return null;

            return new PermissionItemDto
            {
                Id = entity.Id,
                PermissionCode = entity.PermissionCode,
                Name = entity.PermissionName
            };
        }
    }
}
