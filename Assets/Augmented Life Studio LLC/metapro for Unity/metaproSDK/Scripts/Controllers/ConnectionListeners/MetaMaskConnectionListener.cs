using System;
using MetaMask.Models;
using MetaMask.Transports.Unity;
using metaproSDK.Scripts.Utils;
using UnityEngine;

namespace metaproSDK.Scripts.Controllers.ConnectionListeners
{
    public class MetaMaskConnectionListener : MonoBehaviour, IMetaMaskUnityTransportListener
    {
        public void OnMetaMaskConnectRequest(string url)
        {
            var qrCodeSprite = QRCodeImamgeHandler.GenerateQRCode(url);
            Application.OpenURL($"https://metamask.app.link/dapp/wc?uri={url}");
            PluginManager.Instance.ShowQRCodeScreen(qrCodeSprite);
        }

        public void OnMetaMaskRequest(string id, MetaMaskEthereumRequest request)
        {
        }

        public void OnMetaMaskFailure(Exception error)
        {
        }

        public void OnMetaMaskSuccess()
        {
        }
    }
}