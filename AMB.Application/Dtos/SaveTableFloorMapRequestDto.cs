using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class SaveTableFloorMapRequestDto
    {
        public List<TableFloorMapShapeRequestDto> Shapes { get; set; } = new();
    }

    public class TableFloorMapShapeRequestDto
    {
        public ShapeType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Rotation { get; set; }
        public string Fill { get; set; } = "hsl(0, 0%, 90%)";
        public int? AssignedTableId { get; set; }
    }
}