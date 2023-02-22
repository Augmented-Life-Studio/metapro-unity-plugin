using System;

namespace metaproSDK.Scripts.SafeTransferFrom.Serialization
{
    [Serializable]
    public class BSCApiRespone
    {
        public string status;
        public string message;
        public BSCApiResponeResult result;
    }
}