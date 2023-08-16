using System.Diagnostics;
using ChatApp.Server.Chat;
using MagicOnion.Server.Hubs;

namespace ChatApp.Server.Hubs;

public class ChatAppHub : StreamingHubBase<IChatAppHub, IChatAppHubReceiver>, IChatAppHub
{
    private IGroup room;
    private string username;
    private ChatRoom chatRoom;

    public async Task<bool> JoinAsync(Guid roomId, Guid userId, string username)
    {

        bool result = false;
        this.username = username;

        if (ChatRoom.AllRooms.TryGetValue(roomId, out chatRoom))
        {
            if (chatRoom.Enter(userId))
            {
                room = await Group.AddAsync(roomId.ToString());
                result = true;
            }
        }

        Broadcast(room).OnJoin(this.username);
        return result;
    }

    public async Task LeaveAsync()
    {
        if (await room.GetMemberCountAsync() <= 1)
        {
            ChatRoom.Destroy(chatRoom);
            chatRoom = null;
        }

        await room.RemoveAsync(this.Context);
        Broadcast(room).OnLeave(username);
    }

    public async Task SendMessageAsync(string message)
    {
        Broadcast(room).OnSendMessage(username, message);
        await Task.CompletedTask;
    }


    protected override ValueTask OnDisconnected()
    {
        BroadcastToSelf(room).OnLeave(username);
        return CompletedTask;
    }
}