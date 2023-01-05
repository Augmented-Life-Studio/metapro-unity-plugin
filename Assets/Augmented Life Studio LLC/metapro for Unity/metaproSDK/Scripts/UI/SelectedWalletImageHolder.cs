using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class SelectedWalletImageHolder : MonoBehaviour
    {
        private Image _image;
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _image.sprite = PluginManager.Instance.GetSelectedWalletSprite;
        }
    }
}