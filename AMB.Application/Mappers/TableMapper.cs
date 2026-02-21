using AMB.Application.Dtos;
using AMB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Mappers
{
    public static class TableMapper
    {
        public static Table ToTableEntity(this CreateTableRequestDto dto)
        {
            if (dto == null) return null;

            return new Table
            {
                Capacity = dto.Capacity,
                TableName = dto.TableName,
                IsOnlineBookingEnabled = dto.IsOnlineBookingEnabled,
            };
        }

        public static TableDto ToTableDto(this Table entity)
        {
            if (entity == null) return null;

            return new TableDto
            {
                Id = entity.Id,
                TableName = entity.TableName,
                Capacity = entity.Capacity,
                IsOnlineBookingEnabled = entity.IsOnlineBookingEnabled,
            };
        }
    }
}
