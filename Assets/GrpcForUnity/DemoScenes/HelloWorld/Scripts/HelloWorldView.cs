using GrpcForUnity.Core;
using Helloworld;
using UnityEngine;
using UnityEngine.UI;

namespace GrpcForUnity.DemoScenes.HelloWorld.Scripts
{
    public class HelloWorldView : MonoBehaviour
    {
        public Button StartServerBtn;
        public Button StopServerBtn;
        public Button ConnectServerBtn;
        public Button SendCmdBtn;
        public InputField ClientName;

        private IGrpcServer GrpcServer = new GrpcServerHello();
        private IGrpcClient GrpcClient = new GrpcClientHello();

        private void Start()
        {
            StartServerBtn.onClick.AddListener(StartServer);
            StopServerBtn.onClick.AddListener(StopServer);
            ConnectServerBtn.onClick.AddListener(ConnectServer);
            SendCmdBtn.onClick.AddListener(SendHelloToServer);
        }


        private void StartServer()
        {
            GrpcServer.StartServer();
        }

        private void StopServer()
        {
            GrpcServer.StopServer();
        }

        private void ConnectServer()
        {
            GrpcClient.ConnectServer(GrpcServer.ip, GrpcServer.port);
        }

        private void SendHelloToServer()
        {
            var replyObj = GrpcClient.SendGrpc(new HelloRequest()
            {
                Name = ClientName.text
            });

            if (replyObj is HelloReply reply)
                Debug.Log("Client Received: " + reply.Message);
        }
    }
}