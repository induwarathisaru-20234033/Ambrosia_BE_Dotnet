using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AMB.Application.Hubs
{
    [Authorize(AuthenticationSchemes = "GuestBearer")]
    public class OrderingHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var resIdClaim = Context.User?.FindFirst("res_id")?.Value;
            if (string.IsNullOrWhiteSpace(resIdClaim) || !int.TryParse(resIdClaim, out var resId))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Reservation_{resId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var resIdClaim = Context.User?.FindFirst("res_id")?.Value;
            if (!string.IsNullOrWhiteSpace(resIdClaim) && int.TryParse(resIdClaim, out var resId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Reservation_{resId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
