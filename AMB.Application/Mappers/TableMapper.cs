using AMB.Application.Dtos;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
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

        public static List<TableCanvasShape> ToTableCanvasShapeEntities(this SaveTableFloorMapRequestDto dto)
        {
            if (dto == null || dto.Shapes == null || dto.Shapes.Count == 0)
            {
                return new List<TableCanvasShape>();
            }

            return dto.Shapes.Select(shape => new TableCanvasShape
            {
                Type = (int)shape.Type,
                X = shape.X,
                Y = shape.Y,
                Width = shape.Width,
                Height = shape.Height,
                Rotation = shape.Rotation,
                Fill = shape.Fill,
                AssignedTableId = shape.AssignedTableId
            }).ToList();
        }

        public static GetTableFloorMapResponseDto ToGetTableFloorMapResponseDto(this List<TableCanvasShape> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return new GetTableFloorMapResponseDto();
            }

            return new GetTableFloorMapResponseDto
            {
                Shapes = entities.Select(shape => new TableFloorMapShapeDto
                {
                    Type = (ShapeType)shape.Type,
                    X = shape.X,
                    Y = shape.Y,
                    Width = shape.Width,
                    Height = shape.Height,
                    Rotation = shape.Rotation,
                    Fill = shape.Fill,
                    AssignedTableId = shape.AssignedTableId,
                    AssignedTable = shape.AssignedTable?.ToTableDto()
                }).ToList()
            };
        }
    }
}
