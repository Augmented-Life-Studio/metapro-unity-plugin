using TMPro;
using UnityEngine;

namespace metaproSDK.Scripts.AFP.SafeTransferFrom
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