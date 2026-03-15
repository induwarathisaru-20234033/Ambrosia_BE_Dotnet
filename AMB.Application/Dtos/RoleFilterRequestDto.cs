namespace AMB.Application.Dtos
{
    public class RoleFilterRequestDto : BaseSearchRequestDto
    {
        public string? RoleName { get; set; }
        public string? Description { get; set; }
    }
}
