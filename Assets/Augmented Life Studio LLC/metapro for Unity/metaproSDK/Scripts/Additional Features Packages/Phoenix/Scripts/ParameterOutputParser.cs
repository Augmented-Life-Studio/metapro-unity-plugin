using Nethereum.ABI.FunctionEncoding;
using UnityEngine;

namespace metaproSDK.Scripts.AFP.Phoenix
{
    public static class ParameterOutputParser<T>
    {
        public static T ParseParameter(ParameterOutput parameterOutput)
        {
            if (parameterOutput.Parameter.DecodedType == typeof(T))
            {
                return (T)parameterOutput.Result;
            }

            Debug.LogError($"Parsing error: {parameterOutput.Parameter.DecodedType} can't be parsed to {typeof(T)}. Value: {parameterOutput.Result}");
            return default;
        }
        
    }
}