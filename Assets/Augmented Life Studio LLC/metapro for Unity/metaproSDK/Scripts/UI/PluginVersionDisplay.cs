using System.Collections;
using System.Collections.Generic;
using metaproSDK.Scripts;
using TMPro;
using UnityEngine;

public class PluginVersionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    
    void Start()
    {
        displayText.text = "Plugin v:" + PluginManager.Instance.PluginVersion;
    }
}
