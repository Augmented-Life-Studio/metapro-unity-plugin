using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using metaproSDK.Scripts.AFP.Phoenix.Serialization;
using metaproSDK.Scripts.AFP.Phoenix.UI;
using metaproSDK.Scripts.AFP.SafeTransferFrom;
using metaproSDK.Scripts.AFP.SafeTransferFrom.Serialization;
using metaproSDK.Scripts.Serialization;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Web3;
using Newtonsoft.Json;
using Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

namespace metaproSDK.Scripts.AFP.Phoenix
{
    public class Phoenix : MonoBehaviour
    {
        [SerializeField] private PhoenixQuestPopup questPopup;
        [SerializeField] private string bscApiKey = "N3DZJ2U62UJER17AXJZKD24FS8VS68F93U";

        private string web3UrlTest = "https://data-seed-prebsc-1-s1.binance.org:8545";
        private string _tokenContractAddressTest = "0x91945DCD15b44eB33dE4D00897843C1Ee524fA88"; //testnet
        private string _phoenixAddressTest = "0xd3a4912f174b7904b1Dc00d39842E5CFB22B6112";
        private string _phoenixABITest =
            @"[{""inputs"":[],""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""_tokenContractAddress"",""type"":""address""},{""indexed"":true,""internalType"":""uint256"",""name"":""questId"",""type"":""uint256""},{""indexed"":false,""internalType"":""address"",""name"":""operator"",""type"":""address""},{""indexed"":false,""internalType"":""uint256[]"",""name"":""outputTokenIds"",""type"":""uint256[]""}],""name"":""CreateQuest"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""previousOwner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""newOwner"",""type"":""address""}],""name"":""OwnershipTransferred"",""type"":""event""},{""inputs"":[{""internalType"":""uint256"",""name"":""_questId"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""closeQuest"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_questId"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""completeQuest"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_tokenContractAddress"",""type"":""address""},{""internalType"":""uint256"",""name"":""_totalNumberOfQuests"",""type"":""uint256""},{""internalType"":""uint256[]"",""name"":""_inputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""_outputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""_inputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""_outputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""bool"",""name"":""_multipleParticipation"",""type"":""bool""},{""internalType"":""uint256"",""name"":""_startBlock"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_endBlock"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""create"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""getAllAvailableQuests"",""outputs"":[{""components"":[{""internalType"":""uint256"",""name"":""questId"",""type"":""uint256""},{""internalType"":""address"",""name"":""tokenContractAddress"",""type"":""address""},{""internalType"":""uint256"",""name"":""totalNumberOfQuests"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""numberOfQuestsCompleted"",""type"":""uint256""},{""internalType"":""uint256[]"",""name"":""inputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""inputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenQuantity"",""type"":""uint256[]""},{""internalType"":""address"",""name"":""operator"",""type"":""address""},{""internalType"":""bool"",""name"":""multipleParticipation"",""type"":""bool""},{""internalType"":""uint256"",""name"":""startBlock"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""endBlock"",""type"":""uint256""},{""internalType"":""bool"",""name"":""valid"",""type"":""bool""}],""internalType"":""struct Phoenix.Quest[]"",""name"":"""",""type"":""tuple[]""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_questId"",""type"":""uint256""}],""name"":""getQuestById"",""outputs"":[{""internalType"":""uint256"",""name"":""questId"",""type"":""uint256""},{""internalType"":""address"",""name"":""tokenContractAddress"",""type"":""address""},{""internalType"":""uint256"",""name"":""totalNumberOfQuests"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""numberOfQuestsCompleted"",""type"":""uint256""},{""internalType"":""uint256[]"",""name"":""inputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""inputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenQuantity"",""type"":""uint256[]""},{""internalType"":""address"",""name"":""operator"",""type"":""address""},{""internalType"":""bool"",""name"":""multipleParticipation"",""type"":""bool""},{""internalType"":""uint256"",""name"":""startBlock"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""endBlock"",""type"":""uint256""},{""internalType"":""bool"",""name"":""valid"",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""uint256[]"",""name"":"""",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":"""",""type"":""uint256[]""},{""internalType"":""bytes"",""name"":"""",""type"":""bytes""}],""name"":""onERC1155BatchReceived"",""outputs"":[{""internalType"":""bytes4"",""name"":"""",""type"":""bytes4""}],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""uint256"",""name"":"""",""type"":""uint256""},{""internalType"":""uint256"",""name"":"""",""type"":""uint256""},{""internalType"":""bytes"",""name"":"""",""type"":""bytes""}],""name"":""onERC1155Received"",""outputs"":[{""internalType"":""bytes4"",""name"":"""",""type"":""bytes4""}],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""owner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""renounceOwnership"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""bytes4"",""name"":""interfaceId"",""type"":""bytes4""}],""name"":""supportsInterface"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""newOwner"",""type"":""address""}],""name"":""transferOwnership"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}]";


        private string web3UrlMain = "https://bsc-dataseed1.binance.org/";
        private string _tokenContractAddressMain = "0xa293D68684Be29540838Dc8A0222De0c43c6b5B4"; //mainnet
        private string _phoenixAddressMain = "0xC34901363cAB85274908C38Ea3A3Aa6f072554B0";
        private string _phoenixABIMain =
            @"[{""inputs"":[],""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""_tokenContractAddress"",""type"":""address""},{""indexed"":true,""internalType"":""uint256"",""name"":""questId"",""type"":""uint256""},{""indexed"":false,""internalType"":""address"",""name"":""operator"",""type"":""address""},{""indexed"":false,""internalType"":""uint256[]"",""name"":""outputTokenIds"",""type"":""uint256[]""}],""name"":""CreateQuest"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""previousOwner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""newOwner"",""type"":""address""}],""name"":""OwnershipTransferred"",""type"":""event""},{""inputs"":[{""internalType"":""uint256"",""name"":""_questId"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""closeQuest"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_questId"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""completeQuest"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""_tokenContractAddress"",""type"":""address""},{""internalType"":""uint256"",""name"":""_totalNumberOfQuests"",""type"":""uint256""},{""internalType"":""uint256[]"",""name"":""_inputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""_outputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""_inputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""_outputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""bool"",""name"":""_multipleParticipation"",""type"":""bool""},{""internalType"":""uint256"",""name"":""_startBlock"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_endBlock"",""type"":""uint256""},{""internalType"":""bytes"",""name"":""_data"",""type"":""bytes""}],""name"":""create"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""getAllAvailableQuests"",""outputs"":[{""components"":[{""internalType"":""uint256"",""name"":""questId"",""type"":""uint256""},{""internalType"":""address"",""name"":""tokenContractAddress"",""type"":""address""},{""internalType"":""uint256"",""name"":""totalNumberOfQuests"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""numberOfQuestsCompleted"",""type"":""uint256""},{""internalType"":""uint256[]"",""name"":""inputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""inputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenQuantity"",""type"":""uint256[]""},{""internalType"":""address"",""name"":""operator"",""type"":""address""},{""internalType"":""bool"",""name"":""multipleParticipation"",""type"":""bool""},{""internalType"":""uint256"",""name"":""startBlock"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""endBlock"",""type"":""uint256""},{""internalType"":""bool"",""name"":""valid"",""type"":""bool""}],""internalType"":""struct Phoenix.Quest[]"",""name"":"""",""type"":""tuple[]""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_questId"",""type"":""uint256""}],""name"":""getQuestById"",""outputs"":[{""internalType"":""uint256"",""name"":""questId"",""type"":""uint256""},{""internalType"":""address"",""name"":""tokenContractAddress"",""type"":""address""},{""internalType"":""uint256"",""name"":""totalNumberOfQuests"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""numberOfQuestsCompleted"",""type"":""uint256""},{""internalType"":""uint256[]"",""name"":""inputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenIds"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""inputTokenQuantities"",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":""outputTokenQuantity"",""type"":""uint256[]""},{""internalType"":""address"",""name"":""operator"",""type"":""address""},{""internalType"":""bool"",""name"":""multipleParticipation"",""type"":""bool""},{""internalType"":""uint256"",""name"":""startBlock"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""endBlock"",""type"":""uint256""},{""internalType"":""bool"",""name"":""valid"",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""uint256[]"",""name"":"""",""type"":""uint256[]""},{""internalType"":""uint256[]"",""name"":"""",""type"":""uint256[]""},{""internalType"":""bytes"",""name"":"""",""type"":""bytes""}],""name"":""onERC1155BatchReceived"",""outputs"":[{""internalType"":""bytes4"",""name"":"""",""type"":""bytes4""}],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""uint256"",""name"":"""",""type"":""uint256""},{""internalType"":""uint256"",""name"":"""",""type"":""uint256""},{""internalType"":""bytes"",""name"":"""",""type"":""bytes""}],""name"":""onERC1155Received"",""outputs"":[{""internalType"":""bytes4"",""name"":"""",""type"":""bytes4""}],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""name"":""owner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""renounceOwnership"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""bytes4"",""name"":""interfaceId"",""type"":""bytes4""}],""name"":""supportsInterface"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""address"",""name"":""newOwner"",""type"":""address""}],""name"":""transferOwnership"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}]";

        
        private string _tokenContractAddress => PluginManager.Instance.IsTestnetSelected? _tokenContractAddressTest : _tokenContractAddressMain;
        private string _phoenixAddress => PluginManager.Instance.IsTestnetSelected? _phoenixAddressTest : _phoenixAddressMain;
        private string _phoenixABI => PluginManager.Instance.IsTestnetSelected? _phoenixABITest : _phoenixABIMain;
        private string web3Url => PluginManager.Instance.IsTestnetSelected? web3UrlTest : web3UrlMain;
        
        
        private const string _functionSignature = "setApprovalForAll(address,bool)";
        private const string _completeQuestSignature = "completeQuest(uint256,bytes)";
        
        private PhoenixQuest _selectedQuest;

        private List<PhoenixQuestToken> _inputTokens = new();
        private List<PhoenixQuestToken> _outputTokens = new();

        public PhoenixQuest SelectedQuest => _selectedQuest;
        
        public List<PhoenixQuestToken> InputTokens => _inputTokens;
        public List<PhoenixQuestToken> OutputTokens => _outputTokens;
        
        
        public void OpenQuestPopup()
        {
            StartCoroutine(DownloadTokenData(_selectedQuest.InputTokenIds, true));
            StartCoroutine(DownloadTokenData(_selectedQuest.OutputTokenIds, false));
            questPopup.OpenInitialScreen();
        }
        
        private void Start()
        {
            GetQuestData(1);
        }

        private async void GetQuestData(int questId)
        {
            var web3Test = new Web3(web3Url);
            var contract = web3Test.Eth.GetContract(_phoenixABI, _phoenixAddress);
            var function = contract.GetFunction("getQuestById");
            var result = await function.CallDecodingToDefaultAsync(questId);
            
            var phoenixQuest = PhoenixDecoder.DecodePhoenixQuest(result);
            Debug.Log(phoenixQuest);
            _selectedQuest = phoenixQuest;
            OpenQuestPopup();
        }

        

        public void AllowTokenControlBySC()
        {
            questPopup.ShowTransactionProcessingScreen();
            var transactionHash = "";
            transactionHash += TransactionEncoder.EncodeFunction(_functionSignature);
            var smartContractAddress = new WalletAddress();
            smartContractAddress.value = _phoenixAddress;
            transactionHash += TransactionEncoder.EncodeParam(smartContractAddress);
            transactionHash += TransactionEncoder.EncodeParam(true);
            StartCoroutine(TransactionRequest(transactionHash));
        }

        
         private IEnumerator TransactionRequest(string transactionHashData)
        {
            TransactionData data = new TransactionData();
            Debug.Log(transactionHashData);
            data.from = WalletConnect.ActiveSession.Accounts[0];
            data.to = _tokenContractAddress;
            data.data = transactionHashData;

            var task = Task.Run(async () => await WalletConnect.ActiveSession.EthSendTransaction(data));
            
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted)
            {
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    Debug.LogWarning(innerException.Message);
                }
                questPopup.HideTransactionProcessingScreen();
                yield break;
            }
            Debug.Log(task.Result);

            if (bscApiKey == "")
            {
                Debug.LogWarning("BSC api key not found");
                questPopup.HideTransactionProcessingScreen();
                questPopup.EnableSecondStage();
                yield break;
            }
            
            var txHash = task.Result;

            var bscApi = PluginManager.Instance.IsTestnetSelected ? "https://api-testnet.bscscan.com" : "https://api.bscscan.com";
            
            var requestUrl = $"{bscApi}/api?module=transaction" +
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
                Debug.LogWarning(apiResponse.result.errDescription);
            }
            
            questPopup.EnableSecondStage();
            questPopup.HideTransactionProcessingScreen();
        }
         
         
         public void CompleteQuest()
         {
             questPopup.ShowTransactionProcessingScreen();
             var transactionHash = "";
             transactionHash += TransactionEncoder.EncodeFunction(_completeQuestSignature);
             transactionHash += TransactionEncoder.EncodeParam(_selectedQuest.QuestId);
             transactionHash += TransactionEncoder.EncodeLastCompleteQuestBytes();
             StartCoroutine(CompleteQuestTransactionRequest(transactionHash));
         }
         
         
         private IEnumerator CompleteQuestTransactionRequest(string transactionHashData)
         {
             TransactionData data = new TransactionData();
             Debug.Log(transactionHashData);
             
             data.from = WalletConnect.ActiveSession.Accounts[0];
             data.to = _phoenixAddress;
             data.data = transactionHashData;

             var task = Task.Run(async () => await WalletConnect.ActiveSession.EthSendTransaction(data));
            
             yield return new WaitUntil(() => task.IsCompleted);
             if (task.IsFaulted)
             {
                 foreach (var innerException in task.Exception.InnerExceptions)
                 {
                     Debug.LogWarning(innerException.Message);
                 }
                 questPopup.HideTransactionProcessingScreen();
                 yield break;
             }
             Debug.Log(task.Result);

             if (bscApiKey == "")
             {
                 Debug.LogWarning("BSC api key not found");
                 questPopup.HideTransactionProcessingScreen();
                 questPopup.ShowScreen(PhoenixScreenType.Success);
                 yield break;
             }
            
             var txHash = task.Result;

             var bscApi = PluginManager.Instance.IsTestnetSelected ? "https://api-testnet.bscscan.com" : "https://api.bscscan.com";
            
             var requestUrl = $"{bscApi}/api?module=transaction" +
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
                 Debug.LogWarning(apiResponse.result.errDescription);
             }
             questPopup.HideTransactionProcessingScreen();
             questPopup.ShowScreen(PhoenixScreenType.Success);
         }
        
        private IEnumerator DownloadTokenData(int[] tokenIds, bool inputTokens)
        {
            var nftParams = String.Join('&', tokenIds.Select(p => "tokenIds=" + p).ToList());
            var requestUrl = PluginManager.Instance.ServerRequestUrl + "/ms/nft/v1/tokens?" + nftParams;

            UnityWebRequest getNfts = UnityWebRequest.Get(requestUrl);

            yield return getNfts.SendWebRequest();

            if (getNfts.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(getNfts.error);
                yield break;
            }

            Debug.Log(getNfts.downloadHandler.text);

            if (PluginManager.Instance.IsTestnetSelected)
            {
                var appNftResult = JsonConvert.DeserializeObject<Results<NftUserTokenResultTest>>(getNfts.downloadHandler.text);
                
                foreach (var nftTokensResult in appNftResult.results)
                {
                    var phoenixQuestToken = new PhoenixQuestToken();
                    phoenixQuestToken.imageUrl = nftTokensResult.token.image;
                    phoenixQuestToken.tokenId = nftTokensResult.token._tokenId;
                    if (inputTokens)
                    {
                        phoenixQuestToken.amount = _selectedQuest.InputTokenQuantities[Array.IndexOf(tokenIds, phoenixQuestToken.tokenId)];
                        _inputTokens.Add(phoenixQuestToken);
                    }
                    else
                    {
                        phoenixQuestToken.amount = _selectedQuest.OutputTokenQuantity[Array.IndexOf(tokenIds, phoenixQuestToken.tokenId)];
                        phoenixQuestToken.available = true;
                        _outputTokens.Add(phoenixQuestToken);
                    }
                }
            }
            else
            {
                var appNftResult = JsonConvert.DeserializeObject<Results<NftUserTokensResult>>(getNfts.downloadHandler.text);
                foreach (var nftTokensResult in appNftResult.results)
                {
                    var phoenixQuestToken = new PhoenixQuestToken();
                    phoenixQuestToken.imageUrl = nftTokensResult.token.image;
                    phoenixQuestToken.tokenId = nftTokensResult.token._tokenId;
                    if (inputTokens)
                    {
                        phoenixQuestToken.amount = _selectedQuest.InputTokenQuantities[Array.IndexOf(tokenIds, phoenixQuestToken.tokenId)];
                        _inputTokens.Add(phoenixQuestToken);
                    }
                    else
                    {
                        phoenixQuestToken.amount = _selectedQuest.OutputTokenQuantity[Array.IndexOf(tokenIds, phoenixQuestToken.tokenId)];
                        phoenixQuestToken.available = true;
                        _outputTokens.Add(phoenixQuestToken);
                    }
                }
            }

            foreach (var questToken in _inputTokens)
            {
                StartCoroutine(GetTokenTexture(questToken));
            }
            foreach (var questToken in _outputTokens)
            {
                StartCoroutine(GetTokenTexture(questToken));
            }

        }

        private IEnumerator GetTokenTexture(PhoenixQuestToken phoenixQuestToken)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(phoenixQuestToken.imageUrl);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.error);
                StartCoroutine(GetGifData(phoenixQuestToken));
            }
            else
            {
                Debug.Log("Downloaded Texture");
                Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                phoenixQuestToken.texture = texture;
            }
        }

        private IEnumerator GetGifData(PhoenixQuestToken phoenixQuestToken)
        {
            UnityWebRequest www = UnityWebRequest.Get(phoenixQuestToken.imageUrl);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.error);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Downloaded GIF texture");
                var relativePath = "Assets/downloadedGif" + phoenixQuestToken.tokenId + ".gif";
                ByteArrayToFile(relativePath, www.downloadHandler.data);
                AssetDatabase.ImportAsset(relativePath);
                AssetDatabase.Refresh();
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);

                importer.isReadable = true;
                importer.textureType = TextureImporterType.Sprite;

                TextureImporterSettings importerSettings = new TextureImporterSettings();
                importer.ReadTextureSettings(importerSettings);
                importerSettings.textureType = TextureImporterType.Sprite;
                importerSettings.spriteExtrude = 0;
                importerSettings.spriteGenerateFallbackPhysicsShape = false;
                importerSettings.spriteMeshType = SpriteMeshType.FullRect;
                importerSettings.spriteMode = (int)SpriteImportMode.Single;
                importer.SetTextureSettings(importerSettings);
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.maxTextureSize = 1024; // or whatever
                importer.alphaIsTransparency = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                EditorUtility.SetDirty(importer);

                importer.SaveAndReimport();
                Sprite spriteImage = (Sprite)AssetDatabase.LoadAssetAtPath(relativePath, typeof(Sprite));
                phoenixQuestToken.texture = spriteImage.texture;
#endif
            }
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
    }
}