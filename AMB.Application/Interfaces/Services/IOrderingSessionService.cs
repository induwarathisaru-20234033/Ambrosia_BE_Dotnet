using AMB.Application.Dtos;
using System;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IOrderingSessionService
    {
        Task<CheckTableResponseDto?> CheckTableOccupancyAsync(Guid tableGuid);
        Task<ConfirmSessionResponseDto?> ConfirmSessionAsync(ConfirmSessionRequestDto request);
    }
}
