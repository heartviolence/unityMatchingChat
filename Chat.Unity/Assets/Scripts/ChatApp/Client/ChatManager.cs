﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Client.UI;
using ChatApp.Data;
using ChatApp.Server.Hubs;
using Cysharp.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using UnityEngine;

namespace ChatApp.Client
{
    public class ChatManager : MonoBehaviour, IChatAppHubReceiver
    {
        //채팅로그
        public ObservableCollection<string> ChatLog { get; set; } = new ObservableCollection<string>();

        //통신 취소용 토큰
        private CancellationTokenSource shutdownCancellation;

        //연결채널
        private ChannelBase channel;

        //서버 호출용
        private IChatAppHub streamingClient;

        void Start()
        {
            shutdownCancellation = new CancellationTokenSource();
            channel = GrpcChannel.ForAddress(ServerInfo.ChatServerAddress, new GrpcChannelOptions()
            {
                HttpHandler = new YetAnotherHttpHandler()
                {
                    SkipCertificateVerification = true
                },
                DisposeHttpClient = true
            });
        }
        public async Task<bool> ConnectServerAsync(Guid chatRoomId, Guid userId, string username)
        {
            try
            {
                streamingClient =
                    await StreamingHubClient.ConnectAsync<IChatAppHub, IChatAppHubReceiver>(channel, this,
                        cancellationToken: shutdownCancellation.Token);

                return await streamingClient.JoinAsync(chatRoomId, userId, username);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return false;
        }

        private void OnDisable()
        {
            ExitChatRoom();
        }

        public async Task SendMessageAsync(string message)
        {
            await streamingClient?.SendMessageAsync(message);
        }

        public async void ExitChatRoom()
        {
            ChatSceneUI.instance.GoToMainMenu();
            ChatLog.Clear();



            if (streamingClient != null) await streamingClient.DisposeAsync();
            if (channel != null) await channel.ShutdownAsync();
        }


        public void OnJoin(string username)
        {
            ChatLog.Add($"{username} enter the room");
        }

        public void OnLeave(string username)
        {
            ChatLog.Add($"{username} leave the room");
            ExitChatRoom();
        }

        public void OnSendMessage(string username, string message)
        {
            ChatLog.Add($"{username} : {message}");
        }

        void OnDestroy()
        {
            shutdownCancellation?.Cancel();
            shutdownCancellation?.Dispose();
        }
    }
}