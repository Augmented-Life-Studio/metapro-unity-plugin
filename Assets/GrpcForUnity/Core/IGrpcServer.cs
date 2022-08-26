using System.Collections.Generic;
using Grpc.Core;

namespace GrpcForUnity.Core
{
    public interface IGrpcServer
    {
        string ip { get; set; }
        int port { get; set; }
        void StartServer();
        void StartServer(string ip, int port);
        void StopServer();
        List<ServerServiceDefinition> BindServerServices();
    }
}