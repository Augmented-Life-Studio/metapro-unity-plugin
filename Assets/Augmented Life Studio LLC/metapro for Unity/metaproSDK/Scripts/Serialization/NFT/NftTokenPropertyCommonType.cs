using Newtonsoft.Json;

namespace metaproSDK.Scripts.Serialization
{
    public class NftTokenPropertyCommonType
    {
        [JsonProperty("2d_spec")]
        public NftPropertyResult[] spec_2d;
        [JsonProperty("3d_spec")]
        public NftPropertyResult[] spec_3d;
        public NftPropertyResult[] sound_spec;
        public NftPropertyResult[] standard;
    }
}