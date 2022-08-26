using System;
using System.Threading.Tasks;
using Grpc.Core;
using UnityEngine;

namespace GrpcForUnity.Core
{
    public abstract class GrpcClientBase : IGrpcClient
    {
        public Channel GrpcClientChannel;
        public int MaxTransferSize = 4000000;

        public int GetMaxTransferSize()
        {
            return MaxTransferSize;
        }

        public void ConnectServer(string ip, int port)
        {
            try
            {
                if (GrpcClientChannel != null)
                    DisposeServer();

                GrpcClientChannel = new Channel(ip + ":" + port, ChannelCredentials.Insecure) { };
            }
            catch (Exception e)
            {
                Debug.Log("Grpc Client Connect Error: " + e);
            }

            Debug.Log("Grpc Client Connected: " + ip + ":" + port);
        }

        public async Task<bool> ConnectServerAsync(string ip, int port)
        {
            try
            {
                if (GrpcClientChannel != null)
                    DisposeServer();

                GrpcClientChannel = new Channel(ip + ":" + port, ChannelCredentials.Insecure) { };
                await GrpcClientChannel.ConnectAsync(DateTime.UtcNow.AddSeconds(2));
                Debug.Log("Grpc Client Connected: " + ip + ":" + port);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Grpc Client Connect Error: " + e);
                return false;
            }
        }

        public void DisposeServer()
        {
            if (GrpcClientChannel == null)
            {
                Debug.Log("Error: Client Not Connect");
                return;
            }

            GrpcClientChannel.ShutdownAsync().Wait();
        }

        public virtual object SendGrpc<T>(T info)
        {
            return null;
        }

        public virtual void SendGrpcStream<T, TReply>(T info, Action<TReply> callBackAction)
        {
        }
    }
}