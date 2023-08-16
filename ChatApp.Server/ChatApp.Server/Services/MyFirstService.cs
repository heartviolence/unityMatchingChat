using MagicOnion;
using MagicOnion.Server;
using ChatApp.Shared;
using System;
using Grpc.Core;

namespace ChatApp.Server.Services;

public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
{
    public async UnaryResult<int> SumAsync(int x, int y)
    {
        Console.Write($"Received:{x},{y}");
        return x + y;
    }
}