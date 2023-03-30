using System;
using UnityEngine;

namespace metaproSDK.Scripts.AFP.Phoenix.UI
{
    public class PhoenixScreen : MonoBehaviour
    {
        [SerializeField] private Phoenix phoenix;
        [SerializeField] private PhoenixScreenType phoenixScreenType;
        [SerializeField] private Transform inputTokensParent;
        [SerializeField] private Transform outputTokensParent;
        [SerializeField] private QuestTokenHolder questTokenHolderPrefab;

        public PhoenixScreenType PhoenixScreenType => phoenixScreenType;

        private void Start()
        {
            if (phoenixScreenType == PhoenixScreenType.Initial)
            {
                return;
            }

            foreach (var inputToken in phoenix.InputTokens)
            {
                var questTokenHolder = Instantiate(questTokenHolderPrefab, inputTokensParent);
                questTokenHolder.Initialize(inputToken);
                if (phoenixScreenType == PhoenixScreenType.Success)
                {
                    questTokenHolder.SetupOutputView();
                    continue;
                }
                questTokenHolder.SetupInputView();
            }
            
            foreach (var inputToken in phoenix.OutputTokens)
            {
                var questTokenHolder = Instantiate(questTokenHolderPrefab, outputTokensParent);
                questTokenHolder.Initialize(inputToken);
                questTokenHolder.SetupOutputView();
            }
        }
    }
}