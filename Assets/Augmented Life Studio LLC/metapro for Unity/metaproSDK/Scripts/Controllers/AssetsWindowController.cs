using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WalletConnectSharp.Unity.Models;

namespace metaproSDK.Scripts.Controllers
{
    public class AssetsWindowController : MonoBehaviour
    {
        [Header("User setup")]
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI userAddressText;
        [SerializeField] private Image userChainImage;
        [SerializeField] private Image userAvatarImage;
        [SerializeField] private TextMeshProUGUI userAssetsAmount;

        [Header("Screen setup")] 
        [SerializeField] private GameObject assetsListObject;
        [SerializeField] private GameObject nftCardObject;
        [SerializeField] private GameObject backButtonObject;
        [SerializeField] private GameObject categoryScrollObject;
        [SerializeField] private TextMeshProUGUI categoryNameText;
        [SerializeField] private TextMeshProUGUI onScrollCategoryNameText;

        [Header("NFTs setup")] 
        [SerializeField] private GameObject userNftsSection;

        [SerializeField] private GameObject appNftsSection;
        [SerializeField] private ObjectColorsHolder userNftsTabScroll;
        [SerializeField] private ObjectColorsHolder appNftsTabScroll;

        [SerializeField] private Transform userNftObjectsParent;
        [SerializeField] private Transform appNftObjectsParent;
        [SerializeField] private NftTokenHolder nftObjectPrefab;

        private bool _userNftGenerated;
        private bool _appNftGenerated;
        private bool _userNftsVisible;
        private List<NftTokenHolder> userTokenObjects = new List<NftTokenHolder>();
        private List<NftTokenHolder> appTokenObjects = new List<NftTokenHolder>();
        private string _currentCategory = "Category";

        private void Awake()
        {
            _userNftsVisible = true;
        }

        private void OnEnable()
        {
            SetupUserData();
            if (_userNftsVisible)
            {
                ShowUserNFT();
            }
            else
            {
                ShowAppNFT();
            }
        }

        private void SetupUserData()
        {
            var userData = PluginManager.Instance.UserData;
            if (userData.userName == null)
            {
                userNameText.gameObject.SetActive(false);
            }
            else
            {
                userNameText.gameObject.SetActive(true);
                userNameText.text = userData.userName;
            }

            userAddressText.text = userData.wallet;
            userChainImage.sprite = PluginManager.Instance.chainsSprites.First(p => p.type == userData.chain).sprite;
            StartCoroutine(GetTexture(userData.userAvatarURL));
            userAssetsAmount.text = CalculateUserTokenAmount().ToString();
        }

        private int CalculateUserTokenAmount()
        {
            var count = 0;
            foreach (var nftTokenData in PluginManager.Instance.userNfts)
            {
                if (PluginManager.Instance.applicationNfts.FirstOrDefault(p => p.tokenId == nftTokenData.tokenId) == null)
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private IEnumerator GetTexture(string url)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            { 
                Debug.Log("Downloaded Texture");
                Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = TextureOperations.TextureToSprite((Texture2D)texture);
                userAvatarImage.sprite = sprite;
            }
        }

        public void ShowUserNFT()
        {
            appNftsSection.SetActive(false);
            userNftsSection.SetActive(true);
            userNftsTabScroll.SetColor(PluginManager.Instance.activeBadgeColor);
            appNftsTabScroll.SetColor(PluginManager.Instance.inActiveBadgeColor);
            if (_userNftGenerated == false)
            {
                GenerateNftObjects(PluginManager.Instance.userNfts, userNftObjectsParent, userTokenObjects);
            }

            FilterNftObjects(userTokenObjects);
            _userNftGenerated = true;
            _userNftsVisible = true;
        }

        public void ShowAppNFT()
        {
            userNftsSection.SetActive(false);
            appNftsSection.SetActive(true);
            userNftsTabScroll.SetColor(PluginManager.Instance.inActiveBadgeColor);
            appNftsTabScroll.SetColor(PluginManager.Instance.activeBadgeColor);
            if (_appNftGenerated == false)
            {
                GenerateNftObjects(PluginManager.Instance.applicationNfts, appNftObjectsParent, appTokenObjects);
            }

            FilterNftObjects(appTokenObjects);
            _appNftGenerated = true;
            _userNftsVisible = false;
        }

        public void RefreshNfts()
        {
            if (_userNftsVisible)
            {
                foreach (Transform child in userNftObjectsParent)
                {
                    Destroy(child.gameObject);
                }
                userTokenObjects = new List<NftTokenHolder>();

                GenerateNftObjects(PluginManager.Instance.userNfts, userNftObjectsParent, userTokenObjects);
                userAssetsAmount.text = CalculateUserTokenAmount().ToString();
                FilterNftObjects(userTokenObjects);
            }
            else
            {
                foreach (Transform child in appNftObjectsParent)
                {
                    Destroy(child.gameObject);
                }
                appTokenObjects = new List<NftTokenHolder>();
                GenerateNftObjects(PluginManager.Instance.applicationNfts, appNftObjectsParent, appTokenObjects);
                FilterNftObjects(appTokenObjects);
            }
        }

        private void GenerateNftObjects(List<NftTokenData> nftsToGenerate, Transform parentObject, List<NftTokenHolder> listToHoldObjects)
        {
            foreach (var nftTokenData in nftsToGenerate)
            {
                if (PluginManager.Instance.applicationNfts.FirstOrDefault(p => p.tokenId == nftTokenData.tokenId) == null)
                {
                    continue;
                }
                var nftObject = Instantiate(nftObjectPrefab, parentObject);
                nftObject.Setup(nftTokenData);
                listToHoldObjects.Add(nftObject);
            }
        }

        public void ShowAssetsList()
        {
            assetsListObject.SetActive(true);
            nftCardObject.SetActive(false);
            backButtonObject.SetActive(false);
        }

        public void ShowNftCard()
        {
            assetsListObject.SetActive(false);
            nftCardObject.SetActive(true);
            backButtonObject.SetActive(true);
        }

        public void OpenCategoryScroll()
        {
            categoryScrollObject.SetActive(true);
            onScrollCategoryNameText.text = _currentCategory;
        }

        public void CloseCategoryScroll()
        {
            categoryScrollObject.SetActive(false);
        }
        
        public void FilterAssetList(string categoryName)
        {
            Debug.Log("Filter list by category: " + categoryName);
            categoryNameText.text = categoryName;
            _currentCategory = categoryName;
            CloseCategoryScroll();

            if (_userNftsVisible)
            {
                FilterNftObjects(userTokenObjects);
            }
            else
            {
                FilterNftObjects(appTokenObjects);
            }
        }

        private void FilterNftObjects(List<NftTokenHolder> listToFilter)
        {
            foreach (var tokenHolder in listToFilter)
            {
                if (tokenHolder.NftTokenData.category == _currentCategory || _currentCategory == "Category")
                {
                    tokenHolder.gameObject.SetActive(true);
                    if (tokenHolder.IsSetupCompleted == false)
                    {
                        tokenHolder.Setup(tokenHolder.NftTokenData);
                    }
                }
                else
                {
                    tokenHolder.gameObject.SetActive(false);
                }
            }
        }
    }
}