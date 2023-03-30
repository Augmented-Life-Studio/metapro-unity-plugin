using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace metaproSDK.Scripts.AFP.Phoenix.UI
{
    public class PhoenixQuestPopup : MonoBehaviour
    {
        [SerializeField] private Phoenix phoenix;
        [SerializeField] private List<PhoenixScreen> phoenixScreens;
        [SerializeField] private List<GameObject> secondStageObjects;
        [SerializeField] private GameObject transactionProcessingObject;
        private PhoenixScreenType _previousPhoenixScreenType;
        private PhoenixScreenType _currentPhoenixScreenType;
        

        public void OpenInitialScreen()
        {
            ShowScreen(PhoenixScreenType.Initial);
        }

        public void OpenQuestScreen()
        {
            var tokensPlayerHas = 0;
            bool hasIncompleteTokens = false;
            foreach (var inputToken in phoenix.InputTokens)
            {
                var userNft = PluginManager.Instance.userNfts.FirstOrDefault(p => p.tokenId == inputToken.tokenId);

                inputToken.available = false;
                if (userNft != null)
                {
                    tokensPlayerHas++;
                    inputToken.playerAmount = userNft.quantity; 
                    if (userNft.quantity >= inputToken.amount)
                    {
                        inputToken.available = true;
                    }
                    else
                    {
                        hasIncompleteTokens = true;
                    }
                }
            }

            if (tokensPlayerHas == phoenix.InputTokens.Count)
            {
                ShowScreen(hasIncompleteTokens
                    ? PhoenixScreenType.SwapInsufficient
                    : PhoenixScreenType.SwapAvailable);
            }
            else if (tokensPlayerHas == 0)
            {
                ShowScreen(PhoenixScreenType.SwapUnavailable);
            }
            else
            {
                ShowScreen(PhoenixScreenType.SwapInsufficient);
            }
        }

        public void ShowScreen(PhoenixScreenType screenType)
        {
            if (screenType == _currentPhoenixScreenType)
            {
                return;
            }

            foreach (var phoenixScreen in phoenixScreens)
            {
                phoenixScreen.gameObject.SetActive(phoenixScreen.PhoenixScreenType == screenType);
            }

            _previousPhoenixScreenType = _currentPhoenixScreenType;
            _currentPhoenixScreenType = screenType;
        }

        public void EnableSecondStage()
        {
            foreach (var secondStageObject in secondStageObjects)
            {
                secondStageObject.SetActive(true);
            }
        }

        public void ShowTransactionProcessingScreen()
        {
            transactionProcessingObject.SetActive(true);
        }
        
        public void HideTransactionProcessingScreen()
        {
            transactionProcessingObject.SetActive(false);
        }

        public void ClosePhoenix()
        {
            Destroy(phoenix.gameObject);
        }

        public void ShowHidePhoenix()
        {
            if (_currentPhoenixScreenType == PhoenixScreenType.None)
            {
                ShowScreen(_previousPhoenixScreenType);
            }
            else
            {
                ShowScreen(PhoenixScreenType.None);
            }
        }
    }
}