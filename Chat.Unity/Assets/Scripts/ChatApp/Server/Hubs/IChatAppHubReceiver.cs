namespace ChatApp.Server.Hubs
{
    public interface IChatAppHubReceiver
    {
         void OnJoin(string userName);

         void OnLeave(string userName);

         void OnSendMessage(string userName, string message);
    }
}