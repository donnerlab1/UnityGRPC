using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using UnityEditor.Experimental.GraphView;

namespace UnityGRPC.Greeter
{
    public class GreeterServerBehaviour : MonoBehaviour
    {
        private Server grpcServer;

        public void Start()
        {
            Debug.Log("Starting Greeter Server...");
            StartServer();
            Debug.Log("Started Greeter Server, waiting for messages...");
        }
    
        public void StartServer()
        {
            grpcServer = new Server()
            {
                Services = { Greeter.BindService(new GreeterServerImplementation())},
                Ports = { new ServerPort("localhost", 30051, ServerCredentials.Insecure)}
            };
            grpcServer.Start();
        }
        
        public void OnApplicationQuit()
        {
            Debug.Log("Stopping Greeter Server..");
            grpcServer.ShutdownAsync().Wait(5000);
            Debug.Log("Stopped Greeter Server");
        }
    }
}

