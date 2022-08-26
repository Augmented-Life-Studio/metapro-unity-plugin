using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [MenuItem("Tools/AppSetup")]
    public static void ShowWindow()
    {
        var window = GetWindow<AppSetupWindow>();
        window.titleContent = new GUIContent("Test app setup");
        window.minSize = new Vector2(500, 600);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/AppSetupTemplate.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);
        
        
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AppSetupStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        
        ShowData();
        
        
        var setupButton = rootVisualElement.Query<Button>("button").First();
        setupButton.clicked += SetupButtonOnclicked;
        // setupButton.clicked += () =>
        // {
        //     DownloadFile("C74929DB-89A1-4401-BD08-88AACBB9FAE7");
        // };
        
        
    }

    private void ShowData()
    {
        var guid = AssetDatabase.FindAssets("t:MetaproAppSetup");
        var setups = new MetaproAppSetup[guid.Length];
        for (int i = 0; i < setups.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid[i]);
            setups[i] = AssetDatabase.LoadAssetAtPath<MetaproAppSetup>(path);
        }

        var infoBox = rootVisualElement.Query<Box>("properties").First();

        // SerializedObject settingsObject = new SerializedObject(setups[0]);
        // SerializedProperty property = settingsObject.GetIterator();
        // property.Next(true);
        // while (property.NextVisible(false))
        // {
        //     PropertyField prop = new PropertyField(property);
        //     
        //     prop.SetEnabled(property.name != "m_Script");
        //     prop.Bind(settingsObject);
        //     infoBox.Add(prop);
        // }
        

    }

    private void SetupButtonOnclicked()
    {
        Debug.Log("Clicked setup button");
        EditorCoroutineUtility.StartCoroutine(GetJSON(), this);
    }

    private void DownloadFile(string bucketHash)
    {
        Channel channel = new Channel("54.36.174.185:8080", ChannelCredentials.Insecure);
        
        var client = new Storage.StorageClient(channel);
        Debug.Log("Bucket hash: " + bucketHash);
        
        var bucketResponse = client.GetBucket(new GetBucketRequest { BucketIdentifier = bucketHash });
        
        foreach (var bucketResponseFile in bucketResponse.Files)
        {
            var fileResponse = client.GetFile(new GetFileRequest { FileIdentifier = bucketResponseFile });
            Debug.Log("Got file: " + fileResponse.FileName);
            var fileBytes = Array.Empty<byte>();
            foreach (var fileResponseChunk in fileResponse.Chunks)
            {
                var fileChunkResponse = client.GetChunk(new GetChunkRequest { ChunkIdentifier = fileResponseChunk });
                fileBytes = CombineBytes(fileBytes, fileChunkResponse.Chunk.ToByteArray());
                
            }
            ByteArrayToFile("/" + fileResponse.FileName, fileBytes);
            Debug.Log("File created");
        }
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

    
    protected IEnumerator GetJSON()
    {
        var appKey = rootVisualElement.Query<TextField>("styled_input").First();
        var appKeyValue = appKey.value;
        
        using (UnityWebRequest www = UnityWebRequest.Get("https://test-api.metaproprotocol.com/ms/apps/v1/apps/appkey/" + appKeyValue))
        {
            yield return www.SendWebRequest();
 
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                var infoBox = rootVisualElement.Query<Box>("properties").First();
                Label label = new Label();
                label.text = www.error;
                infoBox.Add(label);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var infoBox = rootVisualElement.Query<Box>("items").First();
                
                
                var appItemsResult = JsonConvert.DeserializeObject<AppItemsResult>(www.downloadHandler.text);
                foreach (var itemResult in appItemsResult.items)
                {
                    Box itemBox = new Box();
                    itemBox.name = "item";
                    Image itemImage = new Image();
                    itemImage.name = "item-image";
                    Label itemLabel = new Label();
                    
                    Button itemButton = new Button();
                    itemButton.name = "item-button";
                    itemButton.text = "Download";
                    itemButton.clicked += () =>
                    {
                        DownloadFile(itemResult._bucketHash);
                    };

                    
                    itemLabel.name = "item-name";
                    itemLabel.text = itemResult._tokenId.ToString();
                        
                    itemBox.Add(itemImage);
                    itemBox.Add(itemLabel);
                    itemBox.Add(itemButton);
                        
                    infoBox.Add(itemBox);
                }
            }
        }
    }
}
