using System;
using metaproSDK.Scripts.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.Controllers
{
    public class WalletProviderButtonController : MonoBehaviour
    {
        [SerializeField] private Button buttonHolder;
        [SerializeField] private WalletProviderType walletProviderType;

        
        private void Start()
        {
            buttonHolder.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            PluginManager.Instance.ShowProviderLogin(walletProviderType);
        }
    }
}