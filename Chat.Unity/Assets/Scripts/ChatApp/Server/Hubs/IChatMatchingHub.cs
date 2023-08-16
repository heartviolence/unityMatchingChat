using MagicOnion;
using System.Threading.Tasks;
using System;


namespace ChatApp.Server.Hubs
{
    public interface IChatMatchingHub : IStreamingHub<IChatMatchingHub, IChatMatchingHubReceiver>
    {
        Task ChatMatchingStartAsync(string username);
    }
}