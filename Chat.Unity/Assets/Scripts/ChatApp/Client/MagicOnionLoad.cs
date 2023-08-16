using Grpc.Core;
using Grpc.Core.Logging;
using MagicOnion;
using MagicOnion.Unity;
using UnityEngine;

namespace ChatApp.Client
{
    public class MagicOnionLoad
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeInitialize()
        {
            GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new[]
            {
                new ChannelOption("grpc.keepalive_time_ms", 5000),
                new ChannelOption("grpc.keepalive_timeout_ms", 5000),
            }));

            GrpcEnvironment.SetLogger(new UnityDebugLogger());
        }
    }
}