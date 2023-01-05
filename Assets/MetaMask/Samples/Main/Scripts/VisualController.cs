using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace MetaMask.Unity.Samples
{

    public class VisualController : MonoBehaviour
    {
        /// <summary>A test function.</summary>
        public MetaMaskDemo metaMaskTest;
        /// <summary>Determines whether the MetaMask extension is installed.</summary>
        /// <returns>true if the MetaMask extension is installed; otherwise, false.</returns>
        public GameObject welcomeScreen;
        public GameObject mainScreen;
        public Button SignButton;
        public Button TransactionButton;
        public TextMeshProUGUI HeaderText;
        public TextMeshProUGUI DescriptionText;
        private bool scale;

        // Start is called before the first frame update
        private void Start()
        {
            this.metaMaskTest.onWalletConnected += OnWalletConnected;
            this.metaMaskTest.onWalletDisconnected += OnWalletDisconnected;
            this.metaMaskTest.onWalletPaused += OnWalletPaused;
            this.metaMaskTest.onWalletReady += OnWalletReady;
            this.metaMaskTest.onSignSend += OnSignSend;
            this.metaMaskTest.onTransactionSent += OnTransactionSent;
            this.metaMaskTest.onTransactionResult += OnTransactionResult;
        }

        private void OnDisable()
        {
            this.metaMaskTest.onWalletConnected -= OnWalletConnected;
            this.metaMaskTest.onWalletDisconnected += OnWalletDisconnected;
            this.metaMaskTest.onWalletPaused += OnWalletPaused;
            this.metaMaskTest.onWalletReady -= OnWalletReady;
            this.metaMaskTest.onSignSend -= OnSignSend;
            this.metaMaskTest.onTransactionSent -= OnTransactionSent;
        }


        private void OnWalletReady(object sender, EventArgs e)
        {
            WalletStartVisuals();
        }
        private void OnWalletPaused(object sender, EventArgs e)
        {
            WalletStopVisuals();
        }
        public void OpenMainScreen()
        {
            this.welcomeScreen.SetActive(false);
            this.mainScreen.SetActive(true);
        }

        public void OpenWelcomeScreen()
        {
            this.welcomeScreen.SetActive(true);
            this.mainScreen.SetActive(false);
        }

        private void OnWalletConnected(object sender, EventArgs e)
        {
            WalletStartVisuals();
        }

        private void OnWalletDisconnected(object sender, EventArgs e)
        {
            WalletStopVisuals();
            OpenWelcomeScreen();
        }

        private void WalletStartVisuals()
        {
            this.SignButton.interactable = true;
            this.TransactionButton.interactable = true;
            this.HeaderText.text = "Wallet Ready";
            this.DescriptionText.text = "Click on the buttons below to send action";
        }

        private void WalletStopVisuals()
        {
            this.SignButton.interactable = false;
            this.TransactionButton.interactable = false;
            this.HeaderText.text = "Connect Wallet";
            this.DescriptionText.text = "Scan the QR in your MetaMask app";
        }



        private void OnSignSend(object sender, EventArgs e)
        {

            ModalData modalData = new ModalData();
            modalData.headerText = "Sign Sent";
            modalData.bodyText = "Sign has been sent to your wallet, please ensure you have the application open on your device";
            UIModalManager.Instance.OpenModal(modalData);
        }

        private void OnTransactionSent(object sender, EventArgs e)
        {
            ModalData modalData = new ModalData();
            modalData.headerText = "Transaction Sent";
            modalData.bodyText = "Transaction Sent has been sent to your wallet, please ensure you have the application open on your device";
            UIModalManager.Instance.OpenModal(modalData);
        }

        private void OnTransactionResult(object sender, MetaMaskEthereumRequestResultEventArgs e)
        {
            ModalData modalData = new ModalData();
            modalData.type = ModalData.ModalType.Transaction;
            modalData.headerText = "Result Received";
            modalData.bodyText = string.Format("<b>Method Name:</b><br> {0} <br> <br> <b>Transaction Details:</b><br>{1}", e.Request.Method, e.Result.ToString());
            UIModalManager.Instance.OpenModal(modalData);
        }


    }

}