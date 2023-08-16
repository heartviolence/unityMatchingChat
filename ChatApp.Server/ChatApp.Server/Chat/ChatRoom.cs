using System.Collections.Concurrent;
using ChatApp.Server.Hubs;

namespace ChatApp.Server.Chat;

internal class ChatRoom
{
    public class RoomMember
    {
        public enum State
        {
            BeforeEnter,
            AfterEnter,
            Exit
        }

        public Guid UserId { get; set; }

        public string Username { get; set; }
        public State UserState { get; set; } = State.BeforeEnter;
    }


    public Guid RoomId { get; }
    public Dictionary<Guid, RoomMember> Members { get; private set; } = new Dictionary<Guid, RoomMember>();
    public static ConcurrentDictionary<Guid, ChatRoom> AllRooms { get; } = new ConcurrentDictionary<Guid, ChatRoom>();

    private ChatRoom(Guid roomId)
    {
        this.RoomId = roomId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>True:정상동작 , false:이상동작</returns>
    public bool Enter(Guid userId)
    {
        bool result = false;
        if (Members.TryGetValue(userId, out RoomMember member) &&
            member.UserState == RoomMember.State.BeforeEnter)
        {
            member.UserState = RoomMember.State.AfterEnter;
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>True:정상동작 , false:이상동작</returns>
    public bool Exit(Guid userId)
    {
        bool result = false;
        if (Members.TryGetValue(userId, out RoomMember member) &&
            member.UserState == RoomMember.State.AfterEnter)
        {
            member.UserState = RoomMember.State.Exit;
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 방생성후 등록
    /// </summary>
    /// <param name="users"></param>
    /// <returns></returns>
    public static ChatRoom Create(IEnumerable<ChatMatchingData> users)
    {
        ChatRoom room = new ChatRoom(Guid.NewGuid());

        foreach (var userdata in users)
        {
            room.Members.Add(userdata.ContextId, new RoomMember()
            {
                UserId = userdata.ContextId,
                Username = userdata.Username
            });
        }

        AllRooms.TryAdd(room.RoomId, room);

        return room;
    }

    /// <summary>
    /// 방목록 에서 삭제
    /// </summary>
    /// <param name="room"></param>
    public static void Destroy(ChatRoom room)
    {
        AllRooms.TryRemove(room.RoomId, out _);
    }
}