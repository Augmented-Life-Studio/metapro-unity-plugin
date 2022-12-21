using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace metaproSDK.Scripts.Utils
{
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        [SerializeField] private TextMeshProUGUI buttonText;

        [SerializeField] private UnityEvent onButtonClicked;

        public void OnPointerEnter(PointerEventData eventData)
        {
            buttonText.color = activeColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            buttonText.color = inactiveColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            buttonText.color = inactiveColor;
            onButtonClicked.Invoke();
        }
    }
}