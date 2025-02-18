using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class GroupHub : Hub
{
    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task NotifyGroupUpdated(string groupId)
    {
        await Clients.Group(groupId).SendAsync("GroupUpdated");
    }
}
