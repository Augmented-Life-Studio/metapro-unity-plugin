using System.Collections;
using NativeWebSocket;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class PlayerConnection : MonoBehaviour
{
    private MetaproAppSetup _metaproAppSetup;
    private WebSocket websocket;

    public UnityEvent OnLoginRequestBegin;
    public UnityEvent<string> OnLoginRequestSuccess;
    public UnityEvent<string> OnLoginRequestError;
    public UnityEvent OnLoginRequestFinish;
    
    
    private void Start()
    {
        var guid = AssetDatabase.FindAssets("t:MetaproAppSetup");
        var setups = new MetaproAppSetup[guid.Length];
        for (int i = 0; i < setups.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid[i]);
            setups[i] = AssetDatabase.LoadAssetAtPath<MetaproAppSetup>(path);
        }

        _metaproAppSetup = setups[0];
        WSConnection();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }
    public void RequestLogin()
    {
        StartCoroutine(SendCodeRequest(_metaproAppSetup.AppId));
    }
    
    protected IEnumerator SendCodeRequest(string appId)
    {
        OnLoginRequestBegin?.Invoke();
        using (UnityWebRequest www = UnityWebRequest.Get("https://www.google.com"))
        {
            yield return www.SendWebRequest();

            yield return new WaitForSeconds(2);
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                OnLoginRequestError?.Invoke(www.error);
            }
            else
            {
                Debug.Log("Success");
                OnLoginRequestSuccess?.Invoke("123456");
            }
        }
        OnLoginRequestFinish?.Invoke();
    }
    
        
    private async void WSConnection()
    {
        websocket = new WebSocket("ws://localhost:7890/wc");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");

            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages at every 1s
        InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 1f);

        // waiting for messages
        await websocket.Connect();
    }
    
    private async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            // await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            var walletConnectMessage = new WalletConnectMessage();
            walletConnectMessage.topic = "Topic";
            walletConnectMessage.payload = "gimme some payload baby";
            walletConnectMessage.type = "type";
            await websocket.SendText(JsonUtility.ToJson(walletConnectMessage));
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
    
    public class WalletConnectMessage
    {
        public string topic;
        public string payload;
        public string type;
    }
}
