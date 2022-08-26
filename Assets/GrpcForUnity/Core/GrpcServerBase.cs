using System.Collections.Generic;
using Grpc.Core;
using UnityEngine;

namespace GrpcForUnity.Core
{
    public abstract class GrpcServerBase : IGrpcServer
    {
        public string ip { get; set; }
        public int port { get; set; }

        private Server _grpcServer;

        public virtual void StartServer()
        {
            InitializeIpAndPort();
            
            _grpcServer = new Server
            {
                Services = { },
                Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) },
            };

            foreach (var service in BindServerServices())
            {
                _grpcServer.Services.Add(service);
            }
            
            _grpcServer.Start();
            
            Debug.Log("Grpc Server Start At " + ip + ":" + port);
        }

        public void StartServer(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            StartServer();
        }

        public virtual void StopServer()
        {
            _grpcServer.KillAsync().Wait();
            Debug.Log("Grpc Server Stop");
        }

        public abstract List<ServerServiceDefinition> BindServerServices();

        private void InitializeIpAndPort()
        {
            if (ip == null)
            {
                ip = "localhost";
            }

            if (port <= 0)
            {
                port = 50051;
            }
        }
    }
}