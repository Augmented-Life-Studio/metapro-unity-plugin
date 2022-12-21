using System;
using System.Linq;
using metaproSDK.Scripts;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NftWindowController : MonoBehaviour
{
    [Header("Main token details")]
    [SerializeField] private Image tokenImage;

    [SerializeField] private TextMeshProUGUI tokenNameText;
    [SerializeField] private TextMeshProUGUI tokenDetailsText;
    [SerializeField] private Image tokenDetailsChainImage;
    [SerializeField] private Image nftInWalletImage;
    [SerializeField] private TextMeshProUGUI nftInWalletText;

    [Header("Additional details")] 
    [SerializeField] private TextMeshProUGUI detailsIdText;

    [SerializeField] private TextMeshProUGUI detailsSupplyText;
    [SerializeField] private TextMeshProUGUI detailsStandardText;
    [SerializeField] private TextMeshProUGUI detailsChainText;
    [SerializeField] private Image detailsChainImage;
    [SerializeField] private TextMeshProUGUI detailsContractText;

    [Header("Window setup")] 
    [SerializeField] private Color nftInWalletColor;

    [SerializeField] private Color nftNotInWalletColor;

    private void OnEnable()
    {
        if (PluginManager.Instance.selectedNft != null)
        {
            SetupScreenView(PluginManager.Instance.selectedNft);
        }
    }

    public void SetupScreenView(NftTokenData tokenData)
    {
        tokenImage.sprite = TextureOperations.TextureToSprite((Texture2D)tokenData.texture);
        tokenNameText.text = tokenData.tokenName;
        tokenDetailsText.text = tokenData.standard + " | Token ID: " + tokenData.tokenId;
        tokenDetailsChainImage.sprite = PluginManager.Instance.chainsSprites.First(p => p.type == tokenData.chain).sprite;

        detailsIdText.text = tokenData.tokenId.ToString();
        detailsSupplyText.text = tokenData.supply.ToString();
        detailsStandardText.text = tokenData.standard;
        detailsChainText.text = ChainTypeExtension.ChainToString(tokenData.chain);
        detailsChainImage.sprite = tokenDetailsChainImage.sprite;
        detailsContractText.text = tokenData.contract;

        foreach (var nftTokenData in PluginManager.Instance.userNfts)
        {
            if (tokenData.tokenId == nftTokenData.tokenId)
            {
                nftInWalletImage.material.color = nftInWalletColor;
                nftInWalletText.text = "In my wallet";
                break;
            }

            nftInWalletImage.material.color = nftNotInWalletColor;
            nftInWalletText.text = "Not on wallet";
        }
    }
}