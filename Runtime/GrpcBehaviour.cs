using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Events;

namespace UnityGRPC.Runtime
{
    public abstract class GrpcBehaviour : MonoBehaviour
    {
        public CancellationTokenSource cts;
        
        public UnityAction OnUpdate;

        public virtual void Awake()
        {
            cts = new CancellationTokenSource();
        }
        public virtual void Update()
        {
            OnUpdate?.Invoke();
        }

        public virtual void OnApplicationQuit()
        {
            cts.Cancel();
        }
    }
}