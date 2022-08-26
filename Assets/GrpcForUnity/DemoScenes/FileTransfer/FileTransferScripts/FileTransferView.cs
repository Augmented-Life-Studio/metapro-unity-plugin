using System.IO;
using Fileproto;
using GrpcForUnity.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GrpcForUnity.DemoScenes.FileTransfer.FileTransferScripts
{
    public class FileTransferView : MonoBehaviour
    {
        public Button StartServerBtn;
        public Button StopServerBtn;
        public Button ConnectServerBtn;

        public InputField TargetFilePathInput;
        public InputField SaveLocationInput;

        public Text StatusProgressText;
        public Button SendFileToServerBtn;

        private IGrpcServer GrpcServer = new GrpcServerFile();
        private IGrpcClient GrpcClient = new GrpcClientFile();

        private void Start()
        {
            SendFileToServerBtn.onClick.AddListener(SendFileToServer);
            StartServerBtn.onClick.AddListener(GrpcServer.StartServer);
            StopServerBtn.onClick.AddListener(GrpcServer.StopServer);
            ConnectServerBtn.onClick.AddListener(() => { GrpcClient.ConnectServer(GrpcServer.ip, GrpcServer.port); });
            SaveLocationInput.onEndEdit.AddListener((value) => { GlobalFileInfo.FileLocation = value; });
        }

        private void SendFileToServer()
        {
            var fileInfo = new FileInfo(@TargetFilePathInput.text);
            var TotalSize = fileInfo.Length / GrpcClient.GetMaxTransferSize();
            GrpcClient.SendGrpcStream<string, FileReceivedReply>(@TargetFilePathInput.text,
                (reply) => {
                    if (TotalSize < 1)
                    {
                        StatusProgressText.text = "Done!";
                        return;
                    }
                    StatusProgressText.text = "" + (reply.Progress / TotalSize) * 100 + "%";
                });
        }

    }
}