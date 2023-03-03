using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using MetaPod;
using metaproSDK.Scripts.Serialization;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class AppSetupWindow : EditorWindow
{
    private bool _hasKeyAssigned;
    private TemplateContainer baseContainer;
    private StyleSheet baseStyleSheet;
    private TemplateContainer importedContainer;
    private StyleSheet importedStyleSheet;

    private static string PROD_URL = "";        //https://api.metaproprotocol.com/ms/nft
    private static string TEST_URL = "test-";   //https://test-api.metaproprotocol.com/ms/nft

    private string _requestURL = PROD_URL;
    
    private MetaproAppSetup _metaproAppSetup;
    private DropdownField chainDropdown;
    
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
        var editorAssetsPath = "Assets/Augmented Life Studio LLC/metapro for Unity/Editor";
        VisualTreeAsset baseAssetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(editorAssetsPath + "/AppSetupTemplate.uxml");
        baseContainer = baseAssetTree.CloneTree();
        baseStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(editorAssetsPath + "/AppSetupStyles.uss");

        VisualTreeAsset importedAssetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(editorAssetsPath + "/AppSetupImported.uxml");
        importedContainer = importedAssetTree.CloneTree();
        importedStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(editorAssetsPath + "/AppSetupImportedStyles.uss");
        
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
        EditorUtility.SetDirty(_metaproAppSetup); 
        AssetDatabase.SaveAssets();
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
        
        var pasteButton = rootVisualElement.Query<Button>("paste_input").First();
        pasteButton.clicked -= PasteButtonOnClicked;
        pasteButton.clicked += PasteButtonOnClicked;

        chainDropdown = rootVisualElement.Query<DropdownField>("styled_dropdown").First();
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
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.error);
            imageToAssign.visible = false;
            imageToAssign.SetEnabled(false);
            EditorCoroutineUtility.StartCoroutine(GetGifData(url, imageToAssign), this);
        }
        else
        {
            imageToAssign.SetEnabled(true);
            imageToAssign.visible = true;
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            imageToAssign.image = texture;
        }
    }
    
    private IEnumerator GetGifData(string url, Image imageToAssign)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        Debug.Log("Downloading preview GIF for image: " + url);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.error);
        }
        else
        { 
            Debug.Log("Downloaded GIF texture");
            var fileId = url.Split('/')[3];
            var relativePath = "Assets/Resources/downloadedGif_" + fileId + ".gif";
            ByteArrayToFile(relativePath, www.downloadHandler.data);
            AssetDatabase.ImportAsset(relativePath);
            AssetDatabase.Refresh();
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
 
            importer.isReadable = true;
            importer.textureType = TextureImporterType.Sprite;
            
            TextureImporterSettings importerSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(importerSettings);
            importerSettings.textureType = TextureImporterType.Sprite;
            importerSettings.spriteExtrude = 0;
            importerSettings.spriteGenerateFallbackPhysicsShape = false;
            importerSettings.spriteMeshType = SpriteMeshType.FullRect;
            importerSettings.spriteMode = (int)SpriteImportMode.Single;
            importer.SetTextureSettings(importerSettings);
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.maxTextureSize = 1024;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            EditorUtility.SetDirty(importer);
            
            importer.SaveAndReimport();
            Sprite spriteImage = (Sprite)AssetDatabase.LoadAssetAtPath(relativePath, typeof(Sprite));
            imageToAssign.image = spriteImage.texture;
            imageToAssign.SetEnabled(true);
            imageToAssign.visible = true;
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


    private void PasteButtonOnClicked()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.Paste();
        var textField = rootVisualElement.Query<TextField>("styled_input").First();
        textField.value = textEditor.text;
    }
    private void SetupButtonOnclicked()
    {
        EditorCoroutineUtility.StartCoroutine(GetAppData(), this);
    }

    private string DownloadFile(string bucketHash, int tokenId, string itemId)
    {
        var podURL = "prd-pod-1";
        if (_metaproAppSetup.SelectedChain.Contains("Testnet"))
        {
            podURL = "tst-pod-2";
        }
        var channel = new Channel(podURL + ".metaprotocol.one:8181", ChannelCredentials.Insecure);

        var client = new Storage.StorageClient(channel);
        
        var bucketResponse = client.GetBucket(new GetBucketRequest { BucketIdentifier = bucketHash });
       
        var fileURI = "";
        foreach (var bucketResponseFile in bucketResponse.Files)
        {
            var fileResponse = client.GetFile(new GetFileRequest { FileIdentifier = bucketResponseFile });
            
            var fileBytes = Array.Empty<byte>();
            SortedDictionary<ulong, byte[]> fileChunksDictionary = new SortedDictionary<ulong, byte[]>();
            
            foreach (var fileResponseChunk in fileResponse.Chunks)
            {
                var fileChunkResponse = client.GetChunk(new GetChunkRequest { ChunkIdentifier = fileResponseChunk.ChunkIdentifier });
                fileChunksDictionary.Add(fileChunkResponse.ChunkIndex, fileChunkResponse.Chunk.ToByteArray());
            }
            
            foreach (var (key, value) in fileChunksDictionary)
            {
                fileBytes = CombineBytes(fileBytes, value);
            }

            Directory.CreateDirectory("Assets/Resources/" + tokenId);
            ByteArrayToFile("Assets/Resources/" + tokenId + "/" + fileResponse.FileName, fileBytes);
            Debug.Log("Asset download completed");
            fileURI = "Assets/Resources/" + fileResponse.FileName;
            EditorCoroutineUtility.StartCoroutine(SendDownloadConfirmation(_metaproAppSetup.AppId, bucketHash, tokenId, itemId, bucketResponseFile), this);
            
            AssetDatabase.ImportAsset("Assets/Resources/" + tokenId, ImportAssetOptions.ImportRecursive);
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
                Debug.LogError(www.error);
                Debug.LogError(www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
    
    protected IEnumerator GetAppData()
    {
        var appKey = rootVisualElement.Query<TextField>("styled_input").First();
        var appKeyValue = appKey.value;
        _metaproAppSetup.SelectedChain = chainDropdown.value;
        
        _requestURL = PROD_URL;
        if (_metaproAppSetup.SelectedChain.Contains("Testnet"))
        {
            _requestURL = TEST_URL;
        }
        
        using (UnityWebRequest www = UnityWebRequest.Get("https://" + _requestURL + "api.metaproprotocol.com/ms/apps/v1/apps/appkey/" + appKeyValue))
        {
            yield return www.SendWebRequest();
 
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                
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
