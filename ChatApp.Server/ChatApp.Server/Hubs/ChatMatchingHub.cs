using System.Collections.Concurrent;
using ChatApp.Server.Chat;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;


namespace ChatApp.Server.Hubs;

public class ChatMatchingData
{
    static public bool operator ==(ChatMatchingData lhs, ChatMatchingData rhs) =>
        (lhs.ContextId == rhs.ContextId &&
         lhs.Username == rhs.Username);


    static public bool operator !=(ChatMatchingData lhs, ChatMatchingData rhs) => !(lhs == rhs);

    public Guid ContextId { get; set; }
    public string Username { get; set; }
}

[GroupConfiguration(typeof(ConcurrentDictionaryGroupRepositoryFactory))]
public class ChatMatchingHub : StreamingHubBase<IChatMatchingHub, IChatMatchingHubReceiver>, IChatMatchingHub
{
    static private ConcurrentDictionary<Guid, ChatMatchingData> waitUsers =
        new ConcurrentDictionary<Guid, ChatMatchingData>();

    private object matchingLock = new object();

    private IGroup room;
    private const string roomname = "ChatMatchingroom";
    public readonly int matchingCount = 2;

    public async Task ChatMatchingStartAsync(string username)
    {
        Console.WriteLine($"{username}이 접속");
        room = await Group.AddAsync(roomname);

        waitUsers[ConnectionId] = new ChatMatchingData()
        {
            ContextId = Context.ContextId,
            Username = username
        };
        Console.WriteLine($"WaituserCount : {waitUsers.Count}");

        lock (matchingLock)
        {
            var WaitingUserdatas = waitUsers.Take(matchingCount).Select(x => x.Value);
            if (WaitingUserdatas.Count() >= matchingCount)
            {
                ChatRoom chatRoom = ChatRoom.Create(WaitingUserdatas);

                foreach (var userData in WaitingUserdatas)
                {
                    waitUsers.TryRemove(userData.ContextId, out _);
                    BroadcastTo(room, userData.ContextId)
                        .OnMatchingSuccess(chatRoom.RoomId, userData.ContextId, userData.Username);
                }
                Console.WriteLine($"매칭완료 현재 남은 대기인원{waitUsers.Count}명");
            }
        }
    }

    protected override ValueTask OnDisconnected()
    {
        Console.WriteLine("Disconnect");
        lock (matchingLock)
        {
            waitUsers.TryRemove(Context.ContextId, out _);
        }

        return base.OnDisconnected();
    }
}