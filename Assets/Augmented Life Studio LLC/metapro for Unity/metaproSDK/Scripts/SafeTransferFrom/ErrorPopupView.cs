using TMPro;
using UnityEngine;

namespace metaproSDK.Scripts.SafeTransferFrom
{
    public class ErrorPopupView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI errorText;

        public void SetupView(string error)
        {
            errorText.text = error;
        }
    }
}