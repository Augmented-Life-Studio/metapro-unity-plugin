using metaproSDK.Scripts;
using UnityEngine;

public class ChainNetDisplay : MonoBehaviour
{
    [SerializeField] private GameObject testnetIndicator;

    private void Start()
    {
        if (PluginManager.Instance.IsTestnetSelected)
        {
            testnetIndicator.SetActive(true);
        }
        else
        {
            testnetIndicator.SetActive(false);
        }
    }
}