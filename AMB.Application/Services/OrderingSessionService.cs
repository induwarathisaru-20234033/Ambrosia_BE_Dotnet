 using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Services
{
    public class OrderingSessionService : IOrderingSessionService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IConfiguration _configuration;

        public OrderingSessionService(IReservationRepository reservationRepository, IConfiguration configuration)
        {
            _reservationRepository = reservationRepository;
            _configuration = configuration;
        }

        public async Task<CheckTableResponseDto?> CheckTableOccupancyAsync(Guid tableGuid)
        {
            var reservation = await _reservationRepository.GetActiveReservationByTableGuidAsync(tableGuid);

            if (reservation == null || !reservation.ArrivedAt.HasValue)
            {
                return null;
            }

            // Only return the name if ArrivedAt is within the last 4 hours.
            // AND ensure it matches today's date.
            var timeSinceArrival = DateTimeOffset.UtcNow - reservation.ArrivedAt.Value;
            if (timeSinceArrival.TotalHours > 4 || reservation.ArrivedAt.Value.Date != DateTimeOffset.UtcNow.Date)
            {
                // Has been too long or not today (in UTC bounds). 
                return null;
            }

            return new CheckTableResponseDto
            {
                IsOccupied = true,
                DisplayName = MaskName(reservation.CustomerDetail?.Name),
                SessionToken = Guid.NewGuid().ToString("N") // Temporary "Discovery Token" or internal session ID
            };
        }

        public async Task<ConfirmSessionResponseDto?> ConfirmSessionAsync(ConfirmSessionRequestDto request)
        {
            var reservation = await _reservationRepository.GetActiveReservationByTableGuidAsync(request.TableCode);

            if (reservation == null || !reservation.ArrivedAt.HasValue)
            {
                return null;
            }

            if (reservation.ReservationStatus != (int)ReservationStatus.Arrived)
            {
                return null;
            }

            var timeSinceArrival = DateTimeOffset.UtcNow - reservation.ArrivedAt.Value;
            if (timeSinceArrival.TotalHours > 4 || reservation.ArrivedAt.Value.Date != DateTimeOffset.UtcNow.Date)
            {
                return null;
            }

            if (request.ConfirmationType == "Manual")
            {
                if (string.IsNullOrWhiteSpace(request.ReservationCode) || 
                    request.ReservationCode != reservation.ReservationCode)
                {
                    return null;
                }
            }
            else if (request.ConfirmationType != "Implicit")
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["Authentication:GuestSecret"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("GuestSecret is missing in configuration.");
            }
            
            var key = Encoding.ASCII.GetBytes(keyString);
            
            var claims = new[]
            {
                new Claim("res_id", reservation.Id.ToString()),
                new Claim("table_id", reservation.TableId.ToString()),
                new Claim(ClaimTypes.Role, "GuestUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(3),
                Issuer = _configuration["Authentication:GuestIssuer"],
                Audience = _configuration["Authentication:GuestAudience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new ConfirmSessionResponseDto
            {
                Token = tokenHandler.WriteToken(token)
            };
        }

        private static string? MaskName(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return null;

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                // Just return the first name if that's all there is
                return parts[0];
            }

            // Return "First Name + Last Initial" (e.g., "John D.")
            return $"{parts[0]} {parts[parts.Length - 1][0]}.";
        }
    }
}
