using GrpcForUnity.Core;
using Helloworld;

namespace GrpcForUnity.DemoScenes.HelloWorld.Scripts
{
    public class GrpcClientHello : GrpcClientBase
    {
        public override object SendGrpc<TRequest>(TRequest info)
        {
            var client = new Greeter.GreeterClient(GrpcClientChannel);
            var reply = client.SayHello(info as HelloRequest);
            return reply;
        }
        
    }
}