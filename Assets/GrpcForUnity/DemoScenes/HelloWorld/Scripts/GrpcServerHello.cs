using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcForUnity.Core;
using GrpcForUnity.Utility.RuntimeLog;
using Helloworld;

namespace GrpcForUnity.DemoScenes.HelloWorld.Scripts
{
    public class HelloWorldHandler : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            // Your Custom Function To-do when receive request
            RuntimeLogView.SpawnLogs("Server Receive: " + request.Name);
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }

    public class GrpcServerHello : GrpcServerBase
    {
        public override List<ServerServiceDefinition> BindServerServices()
        {
            return new List<ServerServiceDefinition>
            {
                Greeter.BindService(new HelloWorldHandler()),
            };
        }
    }
}