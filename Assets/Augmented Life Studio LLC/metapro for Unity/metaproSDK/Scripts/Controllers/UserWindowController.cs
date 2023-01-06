using System;
using metaproSDK.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.Controllers
{
    public class UserWindowController : MonoBehaviour
    {
        [SerializeField] private GameObject providerWindow;
        [SerializeField] private GameObject qrCodeWindow;
        [SerializeField] private GameObject waitLoginWeb3Window;
        [SerializeField] private AssetsWindowController assetsWindow;

        [SerializeField] private Image qrCodeImage;
        
        private ScreenType _currentScreenType;
        private bool _isScreenOpened;

        private void Start()
        {
            _currentScreenType = ScreenType.Providers;
        }

        public void HideAllScreens()
        {
            providerWindow.SetActive(false);
            qrCodeWindow.SetActive(false);
            waitLoginWeb3Window.SetActive(false);
            assetsWindow.gameObject.SetActive(false);
            _isScreenOpened = false;
        }
        
        public void ShowProviderScreen()
        {
            HideAllScreens();
            providerWindow.SetActive(true);
            _currentScreenType = ScreenType.Providers;
            _isScreenOpened = true;
        }
        
        public void ShowQRCodeScreen(Sprite qrCodeSprite)
        {
            HideAllScreens();
            qrCodeWindow.SetActive(true);
            _currentScreenType = ScreenType.QRCode;
            _isScreenOpened = true;
            qrCodeImage.sprite = qrCodeSprite;
        }
        
        public void ShowLoginWeb3Screen()
        {
            HideAllScreens();
            waitLoginWeb3Window.SetActive(true);
            _currentScreenType = ScreenType.LoginWeb3;
            _isScreenOpened = true;
        }
        
        public void ShowAssetsScreen()
        {
            HideAllScreens();
            assetsWindow.gameObject.SetActive(true);
            assetsWindow.ShowAssetsList();
            _currentScreenType = ScreenType.Assets;
            _isScreenOpened = true;
        }
        
        public void ShowAssetCardScreen()
        {
            HideAllScreens();
            assetsWindow.gameObject.SetActive(true);
            assetsWindow.ShowNftCard();
            _currentScreenType = ScreenType.NFTCard;
            _isScreenOpened = true;
        }

        public void EnableCurrentScreen(bool enable)
        {
            if (enable == false)
            {
                HideAllScreens();
                return;
            }
            switch (_currentScreenType)
            {
                case ScreenType.Providers:
                    ShowProviderScreen();
                    break;
                case ScreenType.QRCode:
                    PluginManager.Instance.ClearCurrentProvider();
                    ShowProviderScreen();
                    break;
                case ScreenType.LoginWeb3:
                    ShowLoginWeb3Screen();
                    break;
                case ScreenType.Assets:
                    ShowAssetsScreen();
                    break;
                case ScreenType.NFTCard:
                    ShowAssetCardScreen();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void ToggleScreen()
        {
            if (_isScreenOpened)
            {
                EnableCurrentScreen(false);
                return;
            }
            EnableCurrentScreen(true);
        }
    }
}