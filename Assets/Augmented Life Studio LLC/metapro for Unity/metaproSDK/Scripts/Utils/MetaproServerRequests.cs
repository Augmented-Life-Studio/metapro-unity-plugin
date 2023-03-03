using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using metaproSDK.Scripts.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace metaproSDK.Scripts.Utils
{
    public class MetaproServerRequests
    {
        private static string prodURL = "https://api.metaproprotocol.com";
        private static string testURL = "https://test-api.coinswap.space";
        
        public static IEnumerator GetLoginHash(string wallet, Action<string> result)
        {
            var baseUrl = PluginManager.Instance.IsTestnetSelected ? testURL : prodURL;
            UnityWebRequest www = UnityWebRequest.Get(baseUrl + "/users-service/auth/signature/hash");
            www.SetRequestHeader("X-Account-wallet", wallet);
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(www.error);
                Debug.LogWarning(www.downloadHandler.error);
                Debug.LogWarning(www.downloadHandler.text);
                result("");
                yield break;
            }
            
            var apiAuthHash = JsonConvert.DeserializeObject<ApiAuthHash>(www.downloadHandler.text);
            if (apiAuthHash == null)
            {
                Debug.LogError("API hash object is null");
                result("");
                yield break;
            }

            var hash = apiAuthHash.hash;

            result(hash);
            yield return hash;
        }

        public static IEnumerator LoginWallet(string wallet, string signature, string hash, Action<UserData> result)
        {
            var loginBody = new ApiLoginBody();
            loginBody.wallet = wallet;
            loginBody.signature = signature;

            var body = JsonConvert.SerializeObject(loginBody);
            byte[] bytes = Encoding.UTF8.GetBytes(body);
            var baseUrl = PluginManager.Instance.IsTestnetSelected ? testURL : prodURL;
            UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/users-service/auth/login", body);
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.SetRequestHeader("X-Account-wallet", wallet);
            www.SetRequestHeader("X-Account-Login-Hash", hash);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(www.error);
                Debug.LogWarning(www.downloadHandler.error);
                Debug.LogWarning(www.downloadHandler.text);
                result(null);
                yield return null;
            }

            var loginResult = JsonConvert.DeserializeObject<ApiLoginResult>(www.downloadHandler.text);

            if (loginResult == null)
            {
                yield return null;
            }
            
            var userData = new UserData();
            userData.userId = loginResult.account.userId;
            userData.wallet = wallet;
            userData.accessToken = loginResult.token.accessToken;
            userData.tokenType = loginResult.token.tokenType;
            
            if (loginResult.account.personalDetails != null)
            {
                userData.userName = loginResult.account.personalDetails.username;
                userData.userBio = loginResult.account.personalDetails.bio;
                userData.userAvatarURL = loginResult.account.personalDetails.avatar;
            }

            result(userData);
            yield return userData;
        }
    }
}