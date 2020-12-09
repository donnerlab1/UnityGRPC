using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using UnityEngine;

namespace UnityGRPC.Greeter
{
    public class GreeterClientBehaviour : MonoBehaviour
    {

        public string greetMessage;
        public bool greetTrigger;

        private Channel Channel;
        private Greeter.GreeterClient Client;
        public void Start()
        {
            CreateClient();
        }

        public void Update()
        {
            if (greetTrigger)
            {
                greetTrigger = false;
                SendMessage();
            }
        }

        public void SendMessage()
        {
            var reply = Client.SayHello(new HelloRequest()
            {
                Name = greetMessage
            });
            Debug.Log(reply.Message);
            
        }

        public void CreateClient()
        {
            Channel = new Channel("localhost:30051", ChannelCredentials.Insecure);
            Client = new Greeter.GreeterClient(Channel);
        }
    }

}
