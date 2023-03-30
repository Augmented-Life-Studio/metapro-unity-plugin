using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;

namespace metaproSDK.Scripts.AFP.SafeTransferFrom
{
    public class TransactionEncoder
    {
        public static string EncodeFunction(string functionWithParams)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(functionWithParams);
            KeccakDigest keccakDigest = new KeccakDigest(256);
            byte[] outputBytes = new byte[keccakDigest.GetDigestSize()];
            keccakDigest.BlockUpdate(inputBytes, 0, inputBytes.Length);
            keccakDigest.DoFinal(outputBytes, 0);
            return Hex.ToHexString(outputBytes)[..8]; //return first 4 bytes of hash 
        }

        private static string SimpleStringEncoding(string input)
        {
            string result = "";

            for (int i = 0; i < 64 - input.Length; i++)
            {
                result += "0";
            }
            result += input;
            return result.ToLower();
        }
        
        public static string EncodeParam(WalletAddress walletAddress)
        {
            return SimpleStringEncoding(walletAddress.value.ToLower().Substring(2));
        }
        
        public static string EncodeParam(int integer)
        {
            string hexValue = integer.ToString("X");
            return SimpleStringEncoding(hexValue);
        }
        
        public static string EncodeParam(bool boolean)
        {
            var integer = boolean ? 1 : 0;
            string hexValue = integer.ToString("X");
            return SimpleStringEncoding(hexValue);
        }

        public static string EncodeLastBytes()
        {
            return "00000000000000000000000000000000000000000000000000000000000000a0" +
                   "0000000000000000000000000000000000000000000000000000000000000001" +
                   "0000000000000000000000000000000000000000000000000000000000000000";
        }
        
        public static string EncodeLastCompleteQuestBytes()
        {
            return "0000000000000000000000000000000000000000000000000000000000000040" +
                   "0000000000000000000000000000000000000000000000000000000000000001" +
                   "0100000000000000000000000000000000000000000000000000000000000000";
        }
    }
}