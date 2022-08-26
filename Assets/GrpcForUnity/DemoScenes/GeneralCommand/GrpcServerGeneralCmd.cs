using System.Collections.Generic;
using System.Threading.Tasks;
using GeneralCommand;
using Grpc.Core;
using GrpcForUnity.Core;
using GrpcForUnity.Utility.RuntimeLog;
using UnityEngine;

namespace GrpcForUnity.DemoScenes.GeneralCommand
{
    public class GeneralCommandHandler : CommandClass.CommandClassBase
    {
        public override Task<CmdReply> SendCommand(CmdRequest request, ServerCallContext context)
        {
            Debug.Log("Server Receive: "+ request.CmdName + request.CmdData);
            RuntimeLogView.SpawnLogs("Server Receive: "+ request.CmdName);
            GeneralCmdView.SetGameImage(request.CmdName, request.CmdData);
            return Task.FromResult(new CmdReply{ CmdName = request.CmdName, CmdReplyData = request.CmdData});
        }
    }
    
    public class GrpcServerGeneralCmd : GrpcServerBase
    {
        public override List<ServerServiceDefinition> BindServerServices()
        {
            return new List<ServerServiceDefinition>
            {
                CommandClass.BindService(new GeneralCommandHandler())
            };
        }
    }
}