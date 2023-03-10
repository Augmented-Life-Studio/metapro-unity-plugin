using System.Collections;
using System.Threading.Tasks;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using UnityEngine;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

namespace metaproSDK.Scripts.Controllers
{
    public class WalletConnectProviderController : ProviderController
    {
        private WalletConnect _walletConnect;
        private bool _applicationPauseStatus;

        private void OnApplicationPause(bool pauseStatus)
        {
            _applicationPauseStatus = pauseStatus;
        }

        public override void Initialize()
        {
            _walletConnect = GetComponent<WalletConnect>();
            _walletConnect.ConnectedEventSession.AddListener(OnConnectionStarted);
            _walletConnect.DisconnectedEvent.AddListener(OnDisconnect);
        }

        private void OnConnectionStarted(WCSessionData arg0)
        {
            if (_applicationPauseStatus && loggedToServer)
            {
                return;
            }
            PluginManager.Instance.OnWalletConnected();
        }

        private void OnDisconnect(WalletConnectUnitySession arg0)
        {
            Debug.LogWarning("User disconnected");
            PluginManager.Instance.DisconnectWallet();
        }
        
        public override void ShowConnection()
        {
            StartCoroutine(DelayConnectionStart());
        }
    
        private IEnumerator DelayConnectionStart()
        {
            yield return new WaitUntil(() => _walletConnect != null && _walletConnect.isConnectionStarted);
            var qrCodeSprite = QRCodeImamgeHandler.GenerateQRCode(_walletConnect.Session.URI);
#if UNITY_ANDROID || UNITY_IOS
            yield return new WaitUntil(() => _walletConnect.Session.ReadyForUserPrompt);
            if (ProviderType == WalletProviderType.METAPRO)
            {
                Application.OpenURL($"metapro://wc?uri={_walletConnect.Session.URI}");
            }
            else
            {
                _walletConnect.OpenDeepLink();
            }
#endif
            PluginManager.Instance.ShowQRCodeScreen(qrCodeSprite);
        }

        public override void RequestSign()
        {
            StartCoroutine(SignLogin());
        }

        private IEnumerator SignLogin()
        {
            yield return new WaitForSeconds(1f);
            if (loggedToServer)
            {
                yield break;
            }
            var wallet = WalletConnect.ActiveSession.Accounts[0].ToLower();
            
            var hash = "";
            yield return  StartCoroutine(MetaproServerRequests.GetLoginHash(wallet, value => hash = value));

            var verifyMessage = $"Please sign to let us verify\nthat you are the owner of this address\n{wallet}\n\nRequest ID {hash}";
            
            var task = Task.Run(async () => await WalletConnect.ActiveSession.EthPersonalSign(wallet, verifyMessage));
            yield return new WaitUntil(() => task.IsCompleted);
            var loginSignature = task.Result;

            var userData = new UserData();
            yield return StartCoroutine(MetaproServerRequests.LoginWallet(wallet, loginSignature, hash, value => userData = value));

            PluginManager.Instance.SetupUserData(userData);
            loggedToServer = true;
        }

        public override void DisconnectWallet()
        {
            WalletConnect.ActiveSession.Disconnect();
            Destroy(gameObject, 1f);
        }
    }
}