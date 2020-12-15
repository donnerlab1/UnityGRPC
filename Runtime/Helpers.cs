using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using UnityEngine;

namespace UnityGRPC.Runtime
{
    public static class GrpcExtensions
    {
        public static async Task ListenUniStream<TResponse>(this AsyncServerStreamingCall<TResponse> call, ConcurrentQueue<TResponse> queue, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested )
            {
                try
                {
                    while (!await call.ResponseStream.MoveNext(ct))
                    {
                        await Task.Delay(10);
                    }
                    queue?.Enqueue(call.ResponseStream.Current);
                }
                catch (RpcException rpc)
                {
                    if (rpc.StatusCode == StatusCode.Cancelled)
                    {
                        return;
                    }
                    Debug.LogException(rpc);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return;
                }
            }
        }
    }

}
