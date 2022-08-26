using System;
using System.Threading.Tasks;

namespace GrpcForUnity.Core
{
    public interface IGrpcClient
    {
        void ConnectServer(string ip, int port);
        Task<bool> ConnectServerAsync(string ip, int port);
        void DisposeServer();
        object SendGrpc<T>(T info);
        void SendGrpcStream<T,TReply>(T info, Action<TReply> callbackAction);

        int GetMaxTransferSize();
    }
}