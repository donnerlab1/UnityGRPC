using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using UnityEngine.Events;

namespace UnityGRPC.Runtime
{
    public class SimpleUniStream<TResult> where TResult : IMessage
    {
        private ConcurrentQueue<TResult> queue;

        public UnityAction<TResult> Action;

        public SimpleUniStream(GrpcBehaviour behaviour, UnityAction<TResult> action)
        {
            queue = new ConcurrentQueue<TResult>();
            behaviour.OnUpdate += CheckForUpdate;
            Action = action;
        }

        public async Task StartListenening(AsyncServerStreamingCall<TResult> client, CancellationToken ct)
        {
            await client.ListenUniStream(queue, ct);
            
        }
        // Call from Unity Main thread(e.g. via Update()) to correctly handle events
        public void CheckForUpdate()
        {
            while (queue.TryDequeue(out var res))
            {
                Action?.Invoke(res);
            }
            
        }
    }
}