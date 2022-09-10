using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grpc.Core;
using MetaPod;
using Newtonsoft.Json;
using Serialization;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Object = System.Object;

public class AppSetupWindow : EditorWindow
{
    private bool _hasKeyAssigned;
    private TemplateContainer baseContainer;
    private StyleSheet baseStyleSheet;
    private TemplateContainer importedContainer;
    private StyleSheet importedStyleSheet;

    private static string TEST_URL = "test-";
    private static string UAT_URL = "uat-";
    private static string PROD_URL = "";

    private string _requestURL = TEST_URL;
    
    private MetaproAppSetup _metaproAppSetup;
    [MenuItem("Tools/Metapro SDK Setup")]
    public static void ShowWindow()
    {
        var window = GetWindow<AppSetupWindow>();
        window.titleContent = new GUIContent("Metapro SDK");
        window.minSize = new Vector2(400, 600);
        window.maxSize = new Vector2(400, 600);
    }

    private void OnEnable()
    {
        
        VisualTreeAsset baseAssetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/AppSetupTemplate.uxml");
        baseContainer = baseAssetTree.CloneTree();
        baseStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AppSetupStyles.uss");

        VisualTreeAsset importedAssetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/AppSetupImported.uxml");
        importedContainer = importedAssetTree.CloneTree();
        importedStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AppSetupImportedStyles.uss");
        
        var guid = AssetDatabase.FindAssets("t:MetaproAppSetup");
        var setups = new MetaproAppSetup[guid.Length];
        for (int i = 0; i < setups.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid[i]);
            setups[i] = AssetDatabase.LoadAssetAtPath<MetaproAppSetup>(path);
        }

        _metaproAppSetup = setups[0];
        
        CheckData();
        UpdateVisuals();
    }

    private void CheckData()
    {
        if (_metaproAppSetup.GameKey != "")
        {
            _hasKeyAssigned = true;
        }
        else
        {
            _hasKeyAssigned = false;
        }
    }

    public void UpdateVisuals()
    {
        if (_hasKeyAssigned)
        {
            EditorCoroutineUtility.StartCoroutine(ShowItemsView(), this);
        }
        else
        {
            EditorCoroutineUtility.StartCoroutine(ShowBaseView(), this);
        }
    }

    private IEnumerator ShowItemsView()
    {
        baseContainer.RemoveFromHierarchy();
        
        rootVisualElement.Add(importedContainer);
        rootVisualElement.styleSheets.Add(importedStyleSheet);

        yield return new WaitForSeconds(1f);
        
        var appName = rootVisualElement.Query<Label>("app_name").First();
        appName.text = _metaproAppSetup.GameName;
        
        var teamName = rootVisualElement.Query<Label>("team_name").First();
        teamName.text = _metaproAppSetup.TeamName;
        
        var appImage = rootVisualElement.Query<Image>("app_image").First();
        
        EditorCoroutineUtility.StartCoroutine(GetTexture(_metaproAppSetup.GameImageURL, appImage), this);

        var listView = rootVisualElement.Query<ScrollView>("test_view").First();
        
        var changeKeyButton = rootVisualElement.Query<Button>("change_app").First();
        changeKeyButton.clicked -= ClearImportedView;
        changeKeyButton.clicked += ClearImportedView;
        
        
        var downloadAllButton = rootVisualElement.Query<Button>("download_all_button").First();
        downloadAllButton.clicked -= DownloadAllItems;
        downloadAllButton.clicked += DownloadAllItems;

        foreach (var gameAsset in _metaproAppSetup.GameAssets)
        {
            
            Box itemBox = new Box();
            itemBox.AddToClassList("item_element");

            Image itemImage = new Image();
            itemImage.AddToClassList("item_image");
            // var file = DownloadFile(gameAsset.ImageURL);
            EditorCoroutineUtility.StartCoroutine(GetTexture(gameAsset.ImageURL, itemImage), this);
            Label itemName = new Label();
            itemName.AddToClassList("item_name");
            itemName.text = gameAsset.Name;
            Button itemButton = new Button();
            itemButton.AddToClassList("item_button");
            itemButton.text = "Download";
            itemButton.clicked += () =>
            {
                DownloadFile(gameAsset.BucketHash, gameAsset.TokenId, gameAsset.ItemId);
            };
            itemBox.Add(itemImage);
            itemBox.Add(itemName);
            itemBox.Add(itemButton);
            listView.Add(itemBox);
        }
    }

    private IEnumerator GetTexture(string url, Image imageToAssign)
    {
        // https://tst-pod-1.metaprotocol.one/dev/0xc9/preview
        Debug.Log("Request texture: " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.error);
            imageToAssign.visible = false;
            imageToAssign.SetEnabled(false);
        }
        else
        {
            imageToAssign.SetEnabled(true);
            imageToAssign.visible = true;
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            imageToAssign.image = texture;
        }
    }

    private void DownloadAllItems()
    {
        foreach (var gameAsset in _metaproAppSetup.GameAssets)
        {
            DownloadFile(gameAsset.BucketHash, gameAsset.TokenId, gameAsset.ItemId);
        }
    }

    private void ClearImportedView()
    {
        var listView = rootVisualElement.Query<ScrollView>("test_view").First();
        _metaproAppSetup.GameKey = "";
        _metaproAppSetup.AppId = "";
        _metaproAppSetup.GameName = "";
        _metaproAppSetup.GameImageURL = "";
        _metaproAppSetup.TeamName = "";
        _metaproAppSetup.GameAssets = new List<AvailableAsset>();
        listView.contentContainer.Clear();
        CheckData();
        UpdateVisuals();
    }

    private IEnumerator ShowBaseView()
    {
        importedContainer.RemoveFromHierarchy();
        rootVisualElement.Add(baseContainer);
        rootVisualElement.styleSheets.Add(baseStyleSheet);

        yield return new WaitForSeconds(1f);
        var textField = rootVisualElement.Query<TextField>("styled_input").First();
        textField.value = "Enter app key";
        
        var setupButton = rootVisualElement.Query<Button>("button").First();
        setupButton.clicked -= SetupButtonOnclicked;
        setupButton.clicked += SetupButtonOnclicked;
    }


    private void SetupButtonOnclicked()
    {
        Debug.Log("Clicked setup button");
        EditorCoroutineUtility.StartCoroutine(GetJSON(), this);
    }

    private string DownloadFile(string bucketHash, int tokenId, string itemId)
    {
        Debug.Log("Clicked setup button");     
        Channel channel = new Channel("tst-pod-1.metaprotocol.one:8181", ChannelCredentials.Insecure);
        Debug.Log("Channel created - " + channel.State);     
        
        var client = new Storage.StorageClient(channel);
        Debug.Log("Bucket hash: " + bucketHash);
        
        var bucketResponse = client.GetBucket(new GetBucketRequest { BucketIdentifier = bucketHash });
       
        var fileURI = "";
        foreach (var bucketResponseFile in bucketResponse.Files)
        {
            var fileResponse = client.GetFile(new GetFileRequest { FileIdentifier = bucketResponseFile });
            Debug.Log("Got file: " + fileResponse.FileName + " - " + bucketResponseFile);
            
            var str = "";
            foreach (var fileResponseChunk in fileResponse.Chunks)
            {
                str += fileResponseChunk.Index + " - " + fileResponseChunk.ChunkIdentifier + "\n";
            }
            Debug.Log(str);
            
            var fileBytes = Array.Empty<byte>();
            SortedDictionary<ulong, byte[]> fileChunksDictionary = new SortedDictionary<ulong, byte[]>();
            
            foreach (var fileResponseChunk in fileResponse.Chunks)
            {
                var fileChunkResponse = client.GetChunk(new GetChunkRequest { ChunkIdentifier = fileResponseChunk.ChunkIdentifier });
                Debug.Log("get chunk - " );
                fileChunksDictionary.Add(fileChunkResponse.ChunkIndex, fileChunkResponse.Chunk.ToByteArray());

            }
            Debug.Log("Finish collecting chunks");
            
            foreach (var (key, value) in fileChunksDictionary)
            {
                Debug.Log("Chunk " + key + " - " + string.Join(", ", value));
                fileBytes = CombineBytes(fileBytes, value);
            }
            Debug.Log("Bytes combined into one array");
            ByteArrayToFile("Assets/Resources/" + fileResponse.FileName, fileBytes);
            fileURI = "Assets/Resources/" + fileResponse.FileName;
            Debug.Log("File created");
            EditorCoroutineUtility.StartCoroutine(SendDownloadConfirmation(
                _metaproAppSetup.AppId, bucketHash, tokenId, itemId, bucketResponseFile), this);

        }

        return fileURI;
    }

    public static byte[] CombineBytes(byte[] first, byte[] second)
    {
        byte[] bytes = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
        Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
        return bytes;
    }
    
    public bool ByteArrayToFile(string fileName, byte[] byteArray)
    {
        try
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(byteArray, 0, byteArray.Length);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in process: {0}", ex);
            return false;
        }
    }

    private IEnumerator SendDownloadConfirmation(
        string appId,
        string bucketHash,
        int tokenId,
        string itemId,
        string fileId)
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", appId);
        form.AddField("_bucketHash", bucketHash);
        form.AddField("_tokenId", tokenId);
        form.AddField("itemId", itemId);
        form.AddField("fileId", fileId);

        using (UnityWebRequest www = UnityWebRequest.Post("https://" + _requestURL + "api.metaproprotocol.com/ms/teams/v1/collect", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
    
    protected IEnumerator GetJSON()
    {
        var appKey = rootVisualElement.Query<TextField>("styled_input").First();
        var appKeyValue = appKey.value;
        
        using (UnityWebRequest www = UnityWebRequest.Get("https://" + _requestURL + "api.metaproprotocol.com/ms/apps/v1/apps/appkey/" + appKeyValue))
        {
            yield return www.SendWebRequest();
 
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var infoBox = rootVisualElement.Query<Box>("items").First();
                
                _metaproAppSetup.GameKey = appKeyValue;
                
                
                var appItemsResult = JsonConvert.DeserializeObject<AppItemsResult>(www.downloadHandler.text);

                _metaproAppSetup.AppId = appItemsResult.appId;
                _metaproAppSetup.GameImageURL = appItemsResult.gallery[0];
                _metaproAppSetup.GameName = appItemsResult.name;
                _metaproAppSetup.TeamName = appItemsResult.team;
                
                _metaproAppSetup.GameAssets = new List<AvailableAsset>();
                
                foreach (var itemResult in appItemsResult.items)
                {
                    AvailableAsset asset = new AvailableAsset();
                    asset.Name = itemResult.tokenName;
                    asset.BucketHash = itemResult._bucketHash;
                    asset.ImageURL = itemResult.image;
                    asset.ItemId = itemResult.itemId;
                    asset.TokenId = itemResult._tokenId;
                    _metaproAppSetup.GameAssets.Add(asset);
                }
                
                CheckData();
                UpdateVisuals();
            }
        }
    }
}
