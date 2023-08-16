using System;
using System.Threading;
using ChatApp.Client.UI;
using ChatApp.Data;
using ChatApp.Server.Hubs;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

namespace ChatApp.Client
{
    public class ChatMatchingManager : MonoBehaviour, IChatMatchingHubReceiver
    {
        [SerializeField] private ChatManager chatManager;

        //통신취소 토큰
        private CancellationTokenSource shutdownCancellation;

        //연결채널
        private ChannelBase channel;

        //서버 rpc호출용
        private IChatMatchingHub streamingClient;

        public async void MatchingAsync(string username)
        {
            try
            {
                shutdownCancellation = new CancellationTokenSource();
                channel = GrpcChannelx.ForAddress(ServerInfo.MatchingServerAddress);

                streamingClient =
                    await StreamingHubClient.ConnectAsync<IChatMatchingHub, IChatMatchingHubReceiver>(channel, this,
                        cancellationToken: shutdownCancellation.Token);

                await streamingClient.ChatMatchingStartAsync(username);
                ChatSceneUI.instance.GoToMatchingWaiting();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public async void DisconnectAsync()
        {
            ChatSceneUI.instance.GoToMainMenu();

            shutdownCancellation.Cancel();

            if (streamingClient != null) await streamingClient.DisposeAsync();
            if (channel != null) await channel.ShutdownAsync();
        }

        public async void OnMatchingSuccess(Guid chatRoomId, Guid contextId, string username)
        {
            bool result = await chatManager.ConnectServerAsync(chatRoomId, contextId, username);

            if (result)
            {
                ChatSceneUI.instance.GoToChatRoom();
            }
        }
    }
}