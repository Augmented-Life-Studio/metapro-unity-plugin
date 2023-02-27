using System;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.AFP.SafeTransferFrom
{
    public class TransferSuccessPopupView : MonoBehaviour
    {
        [Header("Token info")] 
        [SerializeField] private Image tokenImage;
        [SerializeField] private TextMeshProUGUI tokenName;
        [SerializeField] private TextMeshProUGUI tokenID;

        [Header("Transfer info")] 
        [SerializeField] private TextMeshProUGUI toAddressText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Button transactionViewButton;
        
        private NftTokenData _nftTokenData;
        private string _transactionURL;

        public void Setup(NftTokenData nftTokenData, string toAddress, int amount, string transactionURL)
        {
            _nftTokenData = nftTokenData;
            tokenImage.sprite = TextureOperations.TextureToSprite((Texture2D)_nftTokenData.texture);
            tokenName.text = _nftTokenData.tokenName;
            tokenID.text = _nftTokenData.standard + " | Token ID: " + _nftTokenData.tokenId;
            toAddressText.text = toAddress;
            amountText.text = $"{amount}";
            _transactionURL = transactionURL;
        }

        public void OpenBSCScan()
        {
            if (_transactionURL == "")
            {
                return;
            }
            Application.OpenURL(_transactionURL);
        }
    }
}