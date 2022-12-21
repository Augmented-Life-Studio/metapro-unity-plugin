using metaproSDK.Scripts.Utils;
using UnityEngine;

namespace metaproSDK.Scripts.Serialization
{
    public class NftTokenData
    {
        public string imageUrl;
        public Texture texture;
        public string tokenName;
        public string standard;
        public int tokenId;
        public int supply;
        public ChainType chain;
        public string contract;
        public int quantity;
        public string category;
    }
}