using System.Threading.Tasks;
using Grpc.Core;

namespace UnityGRPC.Greeter
{
    public class GreeterServerImplementation : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }
}