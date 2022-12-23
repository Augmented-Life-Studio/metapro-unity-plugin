
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using metaproSDK.Scripts;
using metaproSDK.Scripts.Serialization;
using metaproSDK.Scripts.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NftTokenHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToShow;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;

    private NftTokenData _nftTokenData;
    public NftTokenData NftTokenData => _nftTokenData;

    public bool IsSetupCompleted;
    
    public void HandleClick()
    {
        PluginManager.Instance.ShowAssetCard(_nftTokenData);
    }
    
    public void ShowToken(bool show)
    {
        foreach (var obj in objectsToShow)
        {
            obj.SetActive(show);
        }
    }
    public void Setup(NftTokenData tokenData)
    {
        ShowToken(false);
        _nftTokenData = tokenData;
        nameText.text = tokenData.tokenName;
        amountText.text = tokenData.standard + " | Token ID: " + tokenData.tokenId;
        StartCoroutine(GetTexture(tokenData.imageUrl));
    }

    private IEnumerator GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        Debug.Log("Downloading preview texture for token: " + nameText.text);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.error);
            StartCoroutine(GetGifData(url));
        }
        else
        { 
            Debug.Log("Downloaded Texture");
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            _nftTokenData.texture = texture;
            Sprite sprite = TextureOperations.TextureToSprite((Texture2D)texture);
            image.sprite = sprite;
            ShowToken(true);
            IsSetupCompleted = true;
        }
    }

    private IEnumerator GetGifData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        Debug.Log("Downloading preview GIF for token: " + nameText.text);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.error);
        }
        else
        { 
            Debug.Log("Downloaded GIF texture");
            var relativePath = "Assets/downloadedGif"+ _nftTokenData.tokenId +".gif";
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
            importer.maxTextureSize = 1024; // or whatever
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            EditorUtility.SetDirty(importer);
            
            importer.SaveAndReimport();
            Sprite spriteImage = (Sprite)AssetDatabase.LoadAssetAtPath(relativePath, typeof(Sprite));
            image.sprite = spriteImage;
            _nftTokenData.texture = spriteImage.texture;
            ShowToken(true);
            IsSetupCompleted = true;
        }
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
}
