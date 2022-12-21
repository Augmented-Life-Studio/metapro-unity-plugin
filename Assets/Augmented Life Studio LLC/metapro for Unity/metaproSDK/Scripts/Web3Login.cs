using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using metaproSDK.Scripts;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using Newtonsoft.Json;
using Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Unity;

public class Web3Login : MonoBehaviour
{
    [SerializeField] private GameObject providersScreenObject;
    [SerializeField] private GameObject qrCodeScreenObject;
    [SerializeField] private GameObject walletScreenObject;
    [SerializeField] private NftWindowController nftScreenObject;


    [SerializeField] private Transform tokensParent;
    [SerializeField] private NftTokenHolder tokenPrefab;

    private string _wallet;
    private string _hash;
    private string _loginSignature;
    private List<NftTokenData> _walletTokens;

    public void ShowNftDetails(NftTokenData tokenData)
    {
        nftScreenObject.gameObject.SetActive(true);
        nftScreenObject.SetupScreenView(tokenData);
    }


    void FixedUpdate()
    {
        if (WalletConnect.Instance == null || WalletConnect.ActiveSession.Accounts == null || _wallet != null)
        {
            return;
        }

        _wallet = WalletConnect.ActiveSession.Accounts[0];
        GetWalletTokens();
    }

    public void GetWalletTokens()
    {
        _walletTokens = new List<NftTokenData>();
        var url = "https://api.metaproprotocol.com/ms/nft/v1/user/" + _wallet +
                  "/tokens?_items=true&sort%5Btoken.creationBlock%5D=desc";
        // StartCoroutine(RequestWalletTokens(url));
        // StartCoroutine(SignLogin());
    }

    public void RequestLoginSign()
    {
        StartCoroutine(SignLogin());
    }

    private IEnumerator SignLogin()
    {
        yield return new WaitForSeconds(1f);
        UnityWebRequest www = UnityWebRequest.Get("https://api.metaproprotocol.com/users-service/auth/signature/hash");
        www.SetRequestHeader("X-Account-wallet", _wallet);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            Debug.LogWarning(www.downloadHandler.error);
            yield break;
        }

        var apiAuthHash = JsonConvert.DeserializeObject<ApiAuthHash>(www.downloadHandler.text);
        if (apiAuthHash == null)
        {
            Debug.LogError("API hash object is null");
            yield break;
        }

        _hash = apiAuthHash.hash;

        var verifyMessage = $"Please sign to let us verify\nthat you are the owner of this address\n{_wallet}\n\nRequest ID {_hash}";
        
        var task = Task.Run(async () => await WalletConnect.ActiveSession.EthPersonalSign(_wallet, verifyMessage));
        yield return new WaitUntil(() => task.IsCompleted);
        _loginSignature = task.Result;

        var loginBody = new ApiLoginBody();
        loginBody.wallet = _wallet;
        loginBody.signature = _loginSignature;
        
        var body = JsonConvert.SerializeObject(loginBody);
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        UnityWebRequest login = UnityWebRequest.Post("https://api.metaproprotocol.com/users-service/auth/login", body);
        login.uploadHandler = new UploadHandlerRaw(bytes);
        login.SetRequestHeader("X-Account-wallet", _wallet);
        login.SetRequestHeader("X-Account-Login-Hash", _hash);
        login.SetRequestHeader("Content-Type", "application/json");

        yield return login.SendWebRequest();

        if (login.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(login.error);
            Debug.LogWarning(login.downloadHandler.error);
            Debug.LogWarning(login.downloadHandler.text);
            yield break;
        }

        var loginResult = JsonConvert.DeserializeObject<ApiLoginResult>(login.downloadHandler.text);

        var userData = new UserData();
        userData.userId = loginResult.account.userId;
        userData.wallet = _wallet;
        userData.accessToken = loginResult.token.accessToken;
        userData.tokenType = loginResult.token.tokenType;
        if (loginResult.account.personalDetails != null)
        {
            userData.userName = loginResult.account.personalDetails.username;
            userData.userBio = loginResult.account.personalDetails.bio;
            userData.userAvatarURL = loginResult.account.personalDetails.avatar;
        }
        PluginManager.Instance.SetupUserData(userData);
    }

    

    public async void DisconnectWallet()
    {
        await WalletConnect.ActiveSession.Disconnect();
    }

}
