using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fileproto;
using Grpc.Core;
using GrpcForUnity.Core;

namespace GrpcForUnity.DemoScenes.FileTransfer.FileTransferScripts
{
    public class FileTransferHandler : Fileproto.FileTransfer.FileTransferBase
    {
        private FileStream SourceStream;
        private int ProgressCount = 0;

        public override async Task SendFile(IAsyncStreamReader<FileSendInfo> requestStream,
            IServerStreamWriter<FileReceivedReply> responseStream, ServerCallContext context) {
            if (SourceStream == null)
            {
                SourceStream = File.Open(GlobalFileInfo.FileLocation, FileMode.OpenOrCreate);
            }

            while (await requestStream.MoveNext())
            {
                var fileData = requestStream.Current;
                var result = fileData.FileData.ToByteArray();

                SourceStream.Seek(0, SeekOrigin.End);
                await SourceStream.WriteAsync(result, 0, result.Length);

                await responseStream.WriteAsync(new FileReceivedReply() {
                    Progress = ProgressCount++,
                });
            }

            SourceStream.Close();
            SourceStream = null;
            ProgressCount = 0;
        }
    }

    public class GrpcServerFile : GrpcServerBase
    {
        public override List<ServerServiceDefinition> BindServerServices()
        {
            return new List<ServerServiceDefinition>()
            {
                Fileproto.FileTransfer.BindService(new FileTransferHandler())
            };
        }
    }
}