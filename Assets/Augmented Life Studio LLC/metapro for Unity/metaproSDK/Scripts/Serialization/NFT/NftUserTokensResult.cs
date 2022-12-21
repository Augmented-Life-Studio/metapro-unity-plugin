
namespace metaproSDK.Scripts.Serialization
{
    public class NftUserTokensResult
    {
        public string _id;
        public int chainId;
        public string standard;
        public NftTokenResult token;
        public NftOwnerResult[] owners;
    }
}