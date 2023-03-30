using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using metaproSDK.Scripts.Controllers;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace metaproSDK.Scripts
{
    public class PluginManager : Singleton<PluginManager>
    {
        public readonly string PluginVersion = "1.0.0";

        [SerializeField] private MetaproAppSetup metaproAppSetup;
        [SerializeField] private UserWindowController userWindowController;

        [SerializeField] private List<ProviderController> providerPrefabs;
        
        [Serializable]
        public struct ChainSprite {
            public ChainType type;
            public Sprite sprite;
        }
        public List<ChainSprite> chainsSprites;
        
        [Serializable]
        public struct WalletSprite {
            public WalletProviderType type;
            public Sprite sprite;
        }
        public List<WalletSprite> walletsSprites;
        public Sprite GetSelectedWalletSprite => walletsSprites.First(p => p.type == selectedWalletProvider).sprite;

        public Color activeBadgeColor;
        public Color inActiveBadgeColor;

        public List<NftTokenData> userNfts;
        public List<NftTokenData> applicationNfts;
        private UserData _userData;
        public UserData UserData => _userData;

        public NftTokenData selectedNft;
        private WalletProviderType selectedWalletProvider;
        public WalletProviderType SelectedWalletProvider => selectedWalletProvider;
        private ProviderController _providerController;

        public bool IsTestnetSelected => metaproAppSetup.SelectedChain.Contains("Testnet");

        private string TEST_SERVER_URL = "https://test-api.metaproprotocol.com";
        private string PROD_SERVER_URL = "https://api.metaproprotocol.com";
        public string ServerRequestUrl => IsTestnetSelected ? TEST_SERVER_URL : PROD_SERVER_URL;
        
        private void Start()
        {
            userNfts = new List<NftTokenData>();
            applicationNfts = new List<NftTokenData>();
            StartCoroutine(GetApplicationNFT());
        }


        public void OnWalletConnected()
        {
            userWindowController.ShowLoginWeb3Screen();
            _providerController.RequestSign();
        }

        public void ClearCurrentProvider()
        {
            Destroy(_providerController.gameObject);
            _providerController = null;
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
            
            var url = ServerRequestUrl + "/ms/nft/v1/user/" + _userData.wallet +
                      "/tokens?_items=true&sort%5Btoken.creationBlock%5D=desc";
            StartCoroutine(GetUserNFT(url));
        }
        
        public IEnumerator GetApplicationNFT()
        {
            var getAppAssets = UnityWebRequest.Get(ServerRequestUrl + "/ms/teams/v1/items?appId=" + metaproAppSetup.AppId);

            yield return getAppAssets.SendWebRequest();

            if (getAppAssets.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(getAppAssets.error);
                Debug.LogError(getAppAssets.downloadHandler.text);
                yield break;
            }

            var appAssets = JsonConvert.DeserializeObject<Results<ItemResult>>(getAppAssets.downloadHandler.text);


            var nftParams = String.Join('&', appAssets.results.Select(a => "tokenIds=" + a._tokenId).ToList());
            var requestUrl = ServerRequestUrl + "/ms/nft/v1/tokens?" + nftParams;
            
            UnityWebRequest getNfts = UnityWebRequest.Get(requestUrl);
            
            yield return getNfts.SendWebRequest();

            if (getNfts.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(getNfts.error);
                yield break;
            }
            Debug.Log(getNfts.downloadHandler.text);

            if (IsTestnetSelected)
            {
                var appNftResult = JsonConvert.DeserializeObject<Results<NftUserTokenResultTest>>(getNfts.downloadHandler.text);
                
                foreach (var nftTokensResult in appNftResult.results)
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
                    foreach (var tokenProperty in nftTokensResult.token.properties.common.standard)
                    {
                        if (tokenProperty.key == "asset_category")
                        {
                            nftTokenData.category = (string)tokenProperty.value;
                        }
                    }
                    applicationNfts.Add(nftTokenData);
                }
            }
            else
            {
                var appNftResult = JsonConvert.DeserializeObject<Results<NftUserTokensResult>>(getNfts.downloadHandler.text);
                    
                foreach (var nftTokensResult in appNftResult.results)
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
                if (IsTestnetSelected)
                {
                    var nftTokenResult = JsonConvert.DeserializeObject<Results<NftUserTokenResultTest>>(www.downloadHandler.text);
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
                    
                        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(nftTokenData.imageUrl);
                        yield return textureRequest.SendWebRequest();
                        if (textureRequest.result == UnityWebRequest.Result.Success)
                        {
                            Texture texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
                            nftTokenData.texture = texture;
                        }
                    
                        foreach (var tokenProperty in nftUserTokensResult.token.properties.common.standard)
                        {
                            if (tokenProperty.key == "asset_category")
                            {
                                nftTokenData.category = (string)tokenProperty.value;
                            }
                        }
                    
                        userNfts.Add(nftTokenData);
                    }
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
                    
                        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(nftTokenData.imageUrl);
                        yield return textureRequest.SendWebRequest();
                        if (textureRequest.result == UnityWebRequest.Result.Success)
                        {
                            Texture texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
                            nftTokenData.texture = texture;
                        }
                    
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
            _providerController = Instantiate(providerPrefabs.First(p => p.ProviderType == selectedWalletProvider), transform);
            _providerController.ShowConnection();
        }

        public void ShowQRCodeScreen(Sprite qrCodeSprite)
        {
            userWindowController.ShowQRCodeScreen(qrCodeSprite);
        }

        public void DisconnectWallet()
        {
            _providerController.DisconnectWallet();
            userWindowController.ShowProviderScreen();
        }

    }
}