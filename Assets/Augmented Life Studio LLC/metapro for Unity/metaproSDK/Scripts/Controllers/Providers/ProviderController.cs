using System;
using metaproSDK.Scripts.Serialization;
using UnityEngine;

namespace metaproSDK.Scripts.Controllers
{
    public class ProviderController : MonoBehaviour
    {
        public WalletProviderType ProviderType;
        protected bool loggedToServer;

        private void Start()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            Debug.LogWarning("Implement Initializing for provider: " + ProviderType);
        }
        
        public virtual void ShowConnection()
        {
            Debug.LogWarning("Implement connection for provider: " + ProviderType);
        }
        
        public virtual void RequestSign()
        {
            Debug.LogWarning("Implement signing for provider: " + ProviderType);
        }
        
        public virtual void DisconnectWallet()
        {
            Debug.LogWarning("Implement disconnect for provider: " + ProviderType);
        }
        
    }
}