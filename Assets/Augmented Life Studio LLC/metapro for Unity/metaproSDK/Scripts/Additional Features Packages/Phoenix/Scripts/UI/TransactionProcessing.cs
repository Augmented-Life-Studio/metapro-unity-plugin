using System;
using UnityEngine;

namespace metaproSDK.Scripts.AFP.Phoenix.UI
{
    public class TransactionProcessing : MonoBehaviour
    {
        [SerializeField] private Transform animationImageTransform;

        private void Update()
        {
            animationImageTransform.Rotate(0f, 0, 50 * Time.deltaTime);
        }
    }
}