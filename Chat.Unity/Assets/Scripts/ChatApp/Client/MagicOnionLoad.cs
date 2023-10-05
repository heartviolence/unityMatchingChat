using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Unity;
using UnityEngine;
using Microsoft.Extensions.Logging;

namespace ChatApp.Client
{
    public class MagicOnionLoad
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeInitialize()
        {
            //GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new GrpcChannelOptions()));
        }
    }
}