using System;
using System.Threading;
using ChatApp.Client.UI;
using ChatApp.Data;
using ChatApp.Server.Hubs;
using Cysharp.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
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
        private GrpcChannel channel;

        //서버 rpc호출용
        private IChatMatchingHub streamingClient;

        void Start()
        {
            shutdownCancellation = new CancellationTokenSource();

            channel = GrpcChannel.ForAddress(ServerInfo.MatchingServerAddress, new GrpcChannelOptions()
            {
                HttpHandler = new YetAnotherHttpHandler()
                {
                    SkipCertificateVerification = true
                },
                DisposeHttpClient = true
            });
        }

        public async void MatchingAsync(string username)
        {
            try
            {
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

            await streamingClient.DisposeAsync();

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