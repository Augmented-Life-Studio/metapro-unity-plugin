using System;
using System.IO;
using System.Threading.Tasks;
using Fileproto;
using Google.Protobuf;
using Grpc.Core;
using GrpcForUnity.Core;

namespace GrpcForUnity.DemoScenes.FileTransfer.FileTransferScripts
{
    public class GrpcClientFile : GrpcClientBase
    {
        private Action<FileReceivedReply> callBackActionReply;

        public override async void SendGrpcStream<T, TReply>(T info, Action<TReply> callBackAction)
        {
            var client = new Fileproto.FileTransfer.FileTransferClient(GrpcClientChannel);
            var requests = info as string;
            if (requests == null) return;

            callBackActionReply = callBackAction as Action<FileReceivedReply>;

            using (var grpcCall = client.SendFile())
            {
                SendDataToServer(@requests, grpcCall);
                await StreamResponseFromServer(grpcCall);
            }
        }

        private async Task StreamResponseFromServer(AsyncDuplexStreamingCall<FileSendInfo, FileReceivedReply> grpcCall)
        {
            while (await grpcCall.ResponseStream.MoveNext())
            {
                var replyStatus = grpcCall.ResponseStream.Current;
                callBackActionReply.Invoke(replyStatus);
            }
        }

        private async void SendDataToServer(string filePath,
            AsyncDuplexStreamingCall<FileSendInfo, FileReceivedReply> grpcCall)
        {
            using (Stream source = File.OpenRead(@filePath))
            {
                var buffer = new byte[MaxTransferSize];
                while (await source.ReadAsync(buffer, 0, buffer.Length) > 0)
                {
                    await grpcCall.RequestStream.WriteAsync(new FileSendInfo
                    {
                        FileData = ByteString.CopyFrom(buffer)
                    });
                }
            }

            await grpcCall.RequestStream.CompleteAsync();
        }
    }
}