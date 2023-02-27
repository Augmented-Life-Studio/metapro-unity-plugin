using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.AFP.SafeTransferFrom
{
    public class TransferPopupView : MonoBehaviour
    {
        [Header("Token info")] 
        [SerializeField] private Image tokenImage;
        [SerializeField] private TextMeshProUGUI tokenName;
        [SerializeField] private TextMeshProUGUI tokenID;
        
        [Header("Transfer input")] 
        [SerializeField] private TMP_InputField toAddressInput;
        [SerializeField] private Image toAddressBorderImage;
        [SerializeField] private GameObject toAddressErrorObject;
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TMP_InputField amountInput;
        [SerializeField] private Image amountBorderImage;
        [SerializeField] private GameObject amountErrorObject;
        [SerializeField] private Color initialInputColor;
        [SerializeField] private Color errorInputColor;
        
        private NftTokenData _nftTokenData;

        public string ToAddress => toAddressInput.text;
        public int Amount => int.Parse(amountInput.text);
        public int TokenId => _nftTokenData.tokenId;
    
        public void Setup(NftTokenData nftTokenData)
        {
            _nftTokenData = nftTokenData;
            tokenImage.sprite = TextureOperations.TextureToSprite((Texture2D)_nftTokenData.texture);
            tokenName.text = _nftTokenData.tokenName;
            tokenID.text = _nftTokenData.standard + " | Token ID: " + _nftTokenData.tokenId;
            balanceText.text = $"Balance: {_nftTokenData.quantity}";
        }

        public void CheckinErrors()
        {
            CheckErrors();
        }
        public bool CheckErrors()
        {
            var errorFound = false;
            
            if (toAddressInput.text.Length < 42 || toAddressInput.text.StartsWith("0x") == false)
            {
                errorFound = true;
                toAddressBorderImage.color = errorInputColor;
                toAddressErrorObject.SetActive(true);
            }
            else
            {
                toAddressBorderImage.color = initialInputColor;
                toAddressErrorObject.SetActive(false);
            }

            if (amountInput.text == "" || int.Parse(amountInput.text) == 0)
            {
                errorFound = true;
                amountBorderImage.color = errorInputColor;
                amountErrorObject.SetActive(true);
            }
            else
            {
                amountBorderImage.color = initialInputColor;
                amountErrorObject.SetActive(false);
            }

            if (errorFound == false)
            {
                toAddressBorderImage.color = initialInputColor;
                amountBorderImage.color = initialInputColor;
                toAddressErrorObject.SetActive(false);
                amountErrorObject.SetActive(false);
            }
            
            return errorFound;
        }
    }
    
}