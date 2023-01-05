using System;
using System.Collections;
using System.Threading.Tasks;
using MetaMask.Models;
using MetaMask.Unity;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using UnityEngine;

namespace metaproSDK.Scripts.Controllers
{
    public class MetaMaskProviderController : ProviderController
    {
        private bool _disconnected = false;
        public override void Initialize()
        {
            MetaMaskUnity.Instance.Initialize();
            MetaMaskUnity.Instance.Wallet.WalletAuthorized += OnWalletConnected;
            MetaMaskUnity.Instance.Wallet.WalletDisconnected += OnWalletDisconnected;
        }

        private void OnDestroy()
        {
            MetaMaskUnity.Instance.Wallet.WalletAuthorized -= OnWalletConnected;
            MetaMaskUnity.Instance.Wallet.WalletDisconnected -= OnWalletDisconnected;
        }

        private void OnWalletConnected(object sender, EventArgs e)
        {
            PluginManager.Instance.OnWalletConnected();
        }

        private void OnWalletDisconnected(object sender, EventArgs e)
        {
            _disconnected = true;
            PluginManager.Instance.DisconnectWallet();
        }

        public override void ShowConnection()
        {
            MetaMaskUnity.Instance.Wallet.Connect();
        }

        public override void RequestSign()
        {
            StartCoroutine(SignLogin());
        }

        private IEnumerator SignLogin()
        {
            yield return new WaitForSeconds(1f);
            var wallet = MetaMaskUnity.Instance.Wallet.SelectedAddress.ToLower();

            var hash = "";
            yield return  StartCoroutine(MetaproServerRequests.GetHashss(wallet, value => hash = value));
            
            var verifyMessage = $"Please sign to let us verify\nthat you are the owner of this address\n{wallet}\n\nRequest ID {hash}";
            
            var paramsArray = new[] { wallet, verifyMessage };

            var request = new MetaMaskEthereumRequest
            {
                Method = "personal_sign",
                Parameters = paramsArray
            };
            
            var task = Task.Run(async () => await MetaMaskUnity.Instance.Wallet.Request(request));
            yield return new WaitUntil(() => task.IsCompleted);
            var loginSignature = task.Result;

            var userData = new UserData();
            yield return StartCoroutine(MetaproServerRequests.LoginWallet(wallet, loginSignature.ToString(), hash, value => userData = value));

            PluginManager.Instance.SetupUserData(userData);
        }

        public override void DisconnectWallet()
        {
            if (_disconnected == false)
            {
                MetaMaskUnity.Instance.Wallet.Disconnect();
            }
            Destroy(gameObject, 1f);
        }
    }
    
}