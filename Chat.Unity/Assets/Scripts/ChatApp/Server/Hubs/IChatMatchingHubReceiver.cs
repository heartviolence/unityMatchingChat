using System;
using System.Threading.Tasks;

namespace ChatApp.Server.Hubs
{
    public interface IChatMatchingHubReceiver
    {
        void OnMatchingSuccess(Guid chatRoomId, Guid contextId, string username);
    }
}