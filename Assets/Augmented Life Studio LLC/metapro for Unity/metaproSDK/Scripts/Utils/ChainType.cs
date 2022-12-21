using System;

namespace metaproSDK.Scripts.Utils
{
    public enum ChainType
    {
        BNB, ETH, POLYGON, NONE
    }

    public class ChainTypeExtension
    {
        public static ChainType GetChainById(int chainId)
        {
            if (chainId == 1)
            {
                return ChainType.ETH;
            }
            if (chainId == 56)
            {
                return ChainType.BNB;
            }
            if (chainId == 137)
            {
                return ChainType.POLYGON;
            }

            return ChainType.NONE;
        }

        public static string ChainToString(ChainType chainType)
        {
            switch (chainType)
            {
                case ChainType.BNB:
                    return "BNB Chain";
                case ChainType.ETH:
                    return "ETH Chain";
                case ChainType.POLYGON:
                    return "Polygon";
                default:
                    throw new ArgumentOutOfRangeException(nameof(chainType), chainType, null);
            }
        }
    }
}