using System;
using metaproSDK.Scripts.AFP.Phoenix.Serialization;
using metaproSDK.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.AFP.Phoenix.UI
{
    public class QuestTokenHolder : MonoBehaviour
    {
        [SerializeField] private Image tokenImage;
        [SerializeField] private GameObject tokenUnavailableObject;
        [SerializeField] private TextMeshProUGUI tokenAmountCounterText;

        private PhoenixQuestToken _phoenixQuestToken;
        
        public void Initialize(PhoenixQuestToken phoenixQuestToken)
        {
            _phoenixQuestToken = phoenixQuestToken;
            Sprite sprite = TextureOperations.TextureToSprite((Texture2D)phoenixQuestToken.texture);
            tokenImage.sprite = sprite;
            tokenUnavailableObject.SetActive(!_phoenixQuestToken.available);

        }

        public void SetupInputView()
        {
            var amountText = _phoenixQuestToken.playerAmount + " / " + _phoenixQuestToken.amount;
            tokenAmountCounterText.text = amountText;
        }
        
        public void SetupOutputView()
        {
            var amountText = _phoenixQuestToken.amount.ToString();
            tokenAmountCounterText.text = amountText;
        }
    }
}