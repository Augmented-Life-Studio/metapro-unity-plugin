using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using metaproSDK.Scripts.AFP.Phoenix.Serialization;
using Nethereum.ABI.FunctionEncoding;

namespace metaproSDK.Scripts.AFP.Phoenix
{
    public static class PhoenixDecoder
    {
        public static PhoenixQuest DecodePhoenixQuest(List<ParameterOutput> parameterOutputs)
        {
            var phoenixQuest = new PhoenixQuest();

            phoenixQuest.QuestId = (int)ParameterOutputParser<BigInteger>.ParseParameter(parameterOutputs[0]);
            phoenixQuest.TokenContractAddress = ParameterOutputParser<string>.ParseParameter(parameterOutputs[1]);
            phoenixQuest.TotalNumberOfQuests = (int)ParameterOutputParser<BigInteger>.ParseParameter(parameterOutputs[2]);
            phoenixQuest.NumberOfQuestsCompleted = (int)ParameterOutputParser<BigInteger>.ParseParameter(parameterOutputs[3]);
            phoenixQuest.InputTokenIds = ParameterOutputParser<List<BigInteger>>.ParseParameter(parameterOutputs[4]).Select(p => (int)p).ToArray();
            phoenixQuest.OutputTokenIds = ParameterOutputParser<List<BigInteger>>.ParseParameter(parameterOutputs[5]).Select(p => (int)p).ToArray();
            phoenixQuest.InputTokenQuantities = ParameterOutputParser<List<BigInteger>>.ParseParameter(parameterOutputs[6]).Select(p => (int)p).ToArray();
            phoenixQuest.OutputTokenQuantity = ParameterOutputParser<List<BigInteger>>.ParseParameter(parameterOutputs[7]).Select(p => (int)p).ToArray();
            phoenixQuest.OperatorAddress = ParameterOutputParser<string>.ParseParameter(parameterOutputs[8]);
            phoenixQuest.MultipleParticipation = ParameterOutputParser<bool>.ParseParameter(parameterOutputs[9]);
            phoenixQuest.StartBlock = ParameterOutputParser<BigInteger>.ParseParameter(parameterOutputs[10]);
            phoenixQuest.EndBlock = ParameterOutputParser<BigInteger>.ParseParameter(parameterOutputs[11]);
            phoenixQuest.Valid = ParameterOutputParser<bool>.ParseParameter(parameterOutputs[12]);
            
            return phoenixQuest;
        }
    }
}