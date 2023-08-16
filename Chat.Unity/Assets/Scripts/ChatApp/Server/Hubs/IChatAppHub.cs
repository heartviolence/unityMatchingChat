using System.Threading.Tasks;
using MagicOnion;
using System;

namespace ChatApp.Server.Hubs
{
    public interface IChatAppHub : IStreamingHub<IChatAppHub, IChatAppHubReceiver>
    {
        Task<bool> JoinAsync(Guid roomId, Guid userId, string username);

        Task LeaveAsync();

        Task SendMessageAsync(string message);
    }
}