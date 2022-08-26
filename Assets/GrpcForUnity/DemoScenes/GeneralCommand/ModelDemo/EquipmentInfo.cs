using System;

namespace GrpcForUnity.DemoScenes.GeneralCommand.ModelDemo
{
    public enum EquipmentType
    {
        Armor,
        Helmet,
        Shoes,
        Ring
    }
    
    [Serializable]
    public class EquipmentInfo
    {
        public EquipmentType Type;
        public string Name;
        public int Value;
    }
}