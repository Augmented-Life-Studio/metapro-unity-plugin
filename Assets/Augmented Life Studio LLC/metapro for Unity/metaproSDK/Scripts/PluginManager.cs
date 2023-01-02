using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using metaproSDK.Scripts.Controllers;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using Newtonsoft.Json;
using Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Unity;

namespace metaproSDK.Scripts
{
    public class PluginManager : Singleton<PluginManager>
    {
        [SerializeField] private MetaproAppSetup metaproAppSetup;
        [SerializeField] private Web3Login web3Login;
        [SerializeField] private UserWindowController userWindowController;

        [Serializable]
        public struct ChainSprite {
            public ChainType type;
            public Sprite sprite;
        }
        public List<ChainSprite> chainsSprites;

        public Color activeBadgeColor;
        public Color inActiveBadgeColor;

        public List<NftTokenData> userNfts;
        public List<NftTokenData> applicationNfts;
        private UserData _userData;
        public UserData UserData => _userData;

        public NftTokenData selectedNft;
        private WalletProviderType selectedWalletProvider;

        private void Start()
        {
            userNfts = new List<NftTokenData>();
            applicationNfts = new List<NftTokenData>();
            StartCoroutine(GetApplicationNFT());
            
            // RequestLogin();
        }


        public void RequestLogin()
        {
            userWindowController.ShowLoginWeb3Screen();
            web3Login.RequestLoginSign();
        }

        public void SetupUserData(UserData userData)
        {
            _userData = new UserData();
            _userData.userId = userData.userId;
            _userData.wallet = userData.wallet;
            _userData.userName = userData.userName;
            _userData.userBio = userData.userBio;
            _userData.userAvatarURL = userData.userAvatarURL;
            _userData.accessToken = userData.accessToken;
            _userData.tokenType = userData.tokenType;
            
            var url = "https://api.metaproprotocol.com/ms/nft/v1/user/" + _userData.wallet +
                      "/tokens?_items=true&sort%5Btoken.creationBlock%5D=desc";
            StartCoroutine(GetUserNFT(url));
        }

        public IEnumerator GetApplicationNFT()
        {
            var guid = AssetDatabase.FindAssets("t:MetaproAppSetup");
            if (guid.Length == 0)
            {
                Debug.LogError("MetaproAppSetup not found");
                yield break;
            }

            if (guid.Length > 1)
            {
                Debug.LogError("Only one instance of MetaproAppSetup can persist in project");
                yield break;
            }

            var setups = new MetaproAppSetup[guid.Length];
            for (var i = 0; i < setups.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid[i]);
                setups[i] = AssetDatabase.LoadAssetAtPath<MetaproAppSetup>(path);
            }

            var getAppAssets =
                UnityWebRequest.Get("https://api.metaproprotocol.com/ms/teams/v1/items?appId=" + metaproAppSetup.AppId);

            yield return getAppAssets.SendWebRequest();

            if (getAppAssets.isNetworkError || getAppAssets.isHttpError)
            {
                Debug.LogError(getAppAssets.error);
                Debug.LogError(getAppAssets.downloadHandler.text);
                yield break;
            }

            var appAssets = JsonConvert.DeserializeObject<Results<ItemResult>>(getAppAssets.downloadHandler.text);


            var nftParams = String.Join('&', appAssets.results.Select(a => "tokenIds=" + a._tokenId).ToList());
            var requestUrl = "https://api.metaproprotocol.com/ms/nft/v1/tokens?" + nftParams;
            
            UnityWebRequest getNfts = UnityWebRequest.Get(requestUrl);
            
            yield return getNfts.SendWebRequest();

            if (getNfts.isNetworkError || getNfts.isHttpError)
            {
                Debug.LogError(getNfts.error);
                yield break;
            }
            var appNftResults = JsonConvert.DeserializeObject<Results<NftUserTokensResult>>(getNfts.downloadHandler.text);
            
            foreach (var nftTokensResult in appNftResults.results)
            {
                var nftTokenData = new NftTokenData();
                nftTokenData.tokenId = nftTokensResult.token._tokenId;
                nftTokenData.quantity = nftTokensResult.token._quantity;
                nftTokenData.imageUrl = nftTokensResult.token.image;
                nftTokenData.tokenName = nftTokensResult.token.tokenName;
                nftTokenData.standard = nftTokensResult.standard;
                nftTokenData.contract = nftTokensResult.token.address;
                nftTokenData.supply = nftTokensResult.token._quantity;
                nftTokenData.chain = ChainTypeExtension.GetChainById(nftTokensResult.chainId);
                foreach (var tokenProperty in nftTokensResult.token.properties)
                {
                    if (tokenProperty.key == "asset_category")
                    {
                        nftTokenData.category = (string)tokenProperty.value;
                    }
                }
                applicationNfts.Add(nftTokenData);
            }
        }

        private IEnumerator GetUserNFT(string url)
        {
            yield return new WaitForSeconds(0.5f);
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.error);
            }
            else
            {
                var nftTokenResult = JsonConvert.DeserializeObject<Results<NftUserTokensResult>>(www.downloadHandler.text);
                foreach (var nftUserTokensResult in nftTokenResult.results)
                {
                    var nftTokenData = new NftTokenData();
                    nftTokenData.tokenId = nftUserTokensResult.token._tokenId;
                    if (nftUserTokensResult.owners.Length != 0)
                    {
                        nftTokenData.quantity = nftUserTokensResult.owners[0]._quantity;
                    }

                    nftTokenData.imageUrl = nftUserTokensResult.token.image;
                    nftTokenData.tokenName = nftUserTokensResult.token.tokenName;
                    nftTokenData.standard = nftUserTokensResult.standard;
                    nftTokenData.contract = nftUserTokensResult.token.address;
                    nftTokenData.supply = nftUserTokensResult.token._quantity;
                    nftTokenData.chain = ChainTypeExtension.GetChainById(nftUserTokensResult.chainId);
                    
                    foreach (var tokenProperty in nftUserTokensResult.token.properties)
                    {
                        if (tokenProperty.key == "asset_category")
                        {
                            nftTokenData.category = (string)tokenProperty.value;
                        }
                    }
                    
                    userNfts.Add(nftTokenData);
                }
            }
            userWindowController.ShowAssetsScreen();
        }

        public void ShowAssetCard(NftTokenData nftTokenData)
        {
            selectedNft = nftTokenData;
            userWindowController.ShowAssetCardScreen();
        }

        public void ShowProviderLogin(WalletProviderType walletProviderType)
        {
            selectedWalletProvider = walletProviderType;
            userWindowController.ShowQRCodeScreen();
        }
    }
}