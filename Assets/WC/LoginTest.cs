using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WalletConnectSharp.Unity;

public class LoginTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;

    public void UpdateLoginText(string text)
    {
        codeText.text = text;
    }
}
