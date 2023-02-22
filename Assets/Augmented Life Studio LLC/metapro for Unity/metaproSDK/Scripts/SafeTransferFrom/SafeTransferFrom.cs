using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using metaproSDK.Scripts.SafeTransferFrom.Serialization;
using metaproSDK.Scripts.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

namespace metaproSDK.Scripts.SafeTransferFrom
{
    public class SafeTransferFrom : MonoBehaviour
    {
        [SerializeField] private string bscApiKey;
        [SerializeField] private TransferPopupView transferPopupView;
        [SerializeField] private GameObject signTransactionView;
        [SerializeField] private ErrorPopupView transactionFailedView;
        [SerializeField] private TransferSuccessPopupView transactionSucceedView;
        
        private bool _isOpened;
        
        private const string _smartContractAddress = "0xa293D68684Be29540838Dc8A0222De0c43c6b5B4";
        private const string _functionSignature = "safeTransferFrom(address,address,uint256,uint256,bytes)";

        private SafeTransferFromState _currentState;
        private NftTokenData _nftTokenData;

        private void Start()
        {
            _currentState = SafeTransferFromState.Initial;
        }

        public void SetupTransfer(NftTokenData nftTokenData)
        {
            transferPopupView.Setup(nftTokenData);
            _nftTokenData = nftTokenData;
        }

        public void OpenView()
        {
            HideAllViews();
            switch (_currentState)
            {
                case SafeTransferFromState.Initial:
                    transferPopupView.gameObject.SetActive(true);
                    break;
                case SafeTransferFromState.Transaction_Sent:
                    signTransactionView.gameObject.SetActive(true);
                    break;
                case SafeTransferFromState.Transaction_Failed:
                    transactionFailedView.gameObject.SetActive(true);
                    break;
                case SafeTransferFromState.Transaction_Success:
                    transactionSucceedView.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_currentState), _currentState, null);
            }

            _isOpened = true;
        }
        
        private void HideAllViews()
        {
            transferPopupView.gameObject.SetActive(false);
            signTransactionView.gameObject.SetActive(false);
            transactionFailedView.gameObject.SetActive(false);
            transactionSucceedView.gameObject.SetActive(false);
        }

        public void CloseView()
        {
            HideAllViews();
            _isOpened = false;
        }

        public void OpenClosePopup()
        {
            if (_isOpened)
            {
                CloseView();
            }
            else
            {
                OpenView();
            }
        }

        public void SetState(SafeTransferFromState newState)
        {
            _currentState = newState;
        }

        public void SendSelectedToken()
        {
            if (transferPopupView.CheckErrors())
            {
                return;
            }
            
            var fromAddress = WalletConnect.ActiveSession.Accounts[0];
            var toAddress = transferPopupView.ToAddress;
            var tokenId = transferPopupView.TokenId;
            var amount = transferPopupView.Amount;
            SendToken(fromAddress, toAddress, tokenId, amount);
        }

        public void SendToken(string fromAddress, string toAddress, int tokenId, int amount)
        {
            var transactionHash = "";
            transactionHash += TransactionEncoder.EncodeFunction(_functionSignature);
            var fromWalletAddress = new WalletAddress();
            fromWalletAddress.value = fromAddress;
            transactionHash += TransactionEncoder.EncodeParam(fromWalletAddress);
            var toWalletAddress = new WalletAddress();
            toWalletAddress.value = toAddress;
            transactionHash += TransactionEncoder.EncodeParam(toWalletAddress);
            transactionHash += TransactionEncoder.EncodeParam(tokenId);
            transactionHash += TransactionEncoder.EncodeParam(amount);
            transactionHash += TransactionEncoder.EncodeLastBytes();
            StartCoroutine(TransactionRequest(transactionHash));
        }
        
        private IEnumerator TransactionRequest(string transactionHashData)
        {
            SetState(SafeTransferFromState.Transaction_Sent);
            OpenView();
            TransactionData data = new TransactionData();

            data.from = WalletConnect.ActiveSession.Accounts[0];
            data.to = _smartContractAddress;
            data.data = transactionHashData;

            var task = Task.Run(async () => await WalletConnect.ActiveSession.EthSendTransaction(data));
            
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted)
            {
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    Debug.LogWarning(innerException.Message);
                }
                SetState(SafeTransferFromState.Transaction_Failed);
                transactionFailedView.SetupView(task.Exception.InnerExceptions[0].Message);
                OpenView();
                yield break;
            }
            Debug.Log(task.Result);

            if (bscApiKey == "")
            {
                Debug.LogWarning("BSC api key not found");
                yield break;
            }
            
            var txHash = task.Result;
            var requestUrl = $"https://api.bscscan.com/api?module=transaction" +
                             $"&action=getstatus" +
                             $"&txhash={txHash}" +
                             $"&apikey={bscApiKey}";
            var request = UnityWebRequest.Get(requestUrl);
            yield return request.SendWebRequest();
            
            Debug.Log($"Transaction hash: {request.result}");
            var apiResponse = JsonConvert.DeserializeObject<BSCApiRespone>(request.downloadHandler.text);

            if (apiResponse != null && apiResponse.result.isError == "1")
            {
                Debug.LogWarning("Transaction occured an error");
                SetState(SafeTransferFromState.Transaction_Failed);
                transactionFailedView.SetupView(apiResponse.result.errDescription);
                OpenView();
                yield break;
            }
            
            SetState(SafeTransferFromState.Transaction_Success);
            transactionSucceedView.Setup(_nftTokenData, transferPopupView.ToAddress, transferPopupView.Amount, "https://bscscan.com/tx/" + txHash);
            OpenView();
        }

        public void CloseSafeTransfer()
        {
            Destroy(gameObject);
        }
    }
}