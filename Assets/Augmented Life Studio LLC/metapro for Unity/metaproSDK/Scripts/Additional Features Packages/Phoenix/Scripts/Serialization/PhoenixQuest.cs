using System.Numerics;

namespace metaproSDK.Scripts.AFP.Phoenix.Serialization
{
    public class PhoenixQuest
    {
        public int QuestId;
        public string TokenContractAddress;
        public int TotalNumberOfQuests;
        public int NumberOfQuestsCompleted;
        public int[] InputTokenIds;
        public int[] OutputTokenIds;
        public int[] InputTokenQuantities;
        public int[] OutputTokenQuantity;
        public string OperatorAddress;
        public bool MultipleParticipation;
        public BigInteger StartBlock;
        public BigInteger EndBlock;
        public bool Valid;

        public override string ToString()
        {
            return $"Phoenix quest: {QuestId}, {TokenContractAddress}, {Valid}, {StartBlock} - {EndBlock}";
        }
    }
}