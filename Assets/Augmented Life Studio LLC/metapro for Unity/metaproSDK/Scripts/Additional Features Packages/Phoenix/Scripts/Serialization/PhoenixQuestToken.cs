using UnityEngine;

namespace metaproSDK.Scripts.AFP.Phoenix.Serialization
{
    public class PhoenixQuestToken
    {
        public int tokenId;
        public string imageUrl;
        public Texture texture;
        public int amount;
        public int playerAmount;
        public bool available;

        public override string ToString()
        {
            return $"Quest token: {tokenId} - {amount}. Image: {imageUrl} | {texture}";
        }
    }
}