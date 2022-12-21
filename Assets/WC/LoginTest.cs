using System.Collections;
using System.Collections.Generic;
using B83.Image.GIF;
using metaproSDK.Scripts.Serialization;
using Newtonsoft.Json;
using Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Unity;

public class LoginTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private Transform tokensParent;
    [SerializeField] private NftTokenHolder tokenPrefab;
    
    private string _wallet;

    private List<NftTokenData> _walletTokens;
    
    public void UpdateLoginText(string text)
    {
        codeText.text = text;
    }
    
    void FixedUpdate()
    {
        if (WalletConnect.ActiveSession.Accounts == null)
            return;
        
        _wallet = WalletConnect.ActiveSession.Accounts[0];
    }

    public void GetWalletTokens()
    {
        _walletTokens = new List<NftTokenData>();
        var url = "https://api.metaproprotocol.com/ms/nft/v1/user/" + _wallet + "/tokens?_items=true&sort%5Btoken.creationBlock%5D=desc";
        StartCoroutine(RequestWalletTokens(url));
    }
    
    private IEnumerator RequestWalletTokens(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
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
                _walletTokens.Add(nftTokenData);
            }

            foreach (var walletToken in _walletTokens)
            {
                var tokenHolder = Instantiate(tokenPrefab, tokensParent);
                tokenHolder.Setup(walletToken);
            }
        }
    }
    
    
}
