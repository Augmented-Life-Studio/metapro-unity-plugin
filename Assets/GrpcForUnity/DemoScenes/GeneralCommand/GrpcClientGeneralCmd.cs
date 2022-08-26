using GeneralCommand;
using GrpcForUnity.Core;

namespace GrpcForUnity.DemoScenes.GeneralCommand
{
    public class GrpcClientGeneralCmd : GrpcClientBase
    {
        public override object SendGrpc<T>(T info)
        {
            var client = new CommandClass.CommandClassClient(GrpcClientChannel);
            var reply = client.SendCommand(info as CmdRequest);
            return reply;
        }
    }
}