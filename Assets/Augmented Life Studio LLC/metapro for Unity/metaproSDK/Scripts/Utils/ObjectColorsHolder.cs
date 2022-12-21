using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.Utils
{
    public class ObjectColorsHolder : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectsToColor;

        public void SetColor(Color color)
        {
            foreach (var o in objectsToColor)
            {
                var tmpro = o.GetComponent<TextMeshProUGUI>();
                if (tmpro != null)
                {
                    tmpro.color = color;
                    continue;
                }
                var image = o.GetComponent<Image>();
                if (image != null)
                {
                    image.color = color;
                    continue;
                }
            }
        }
    }
}