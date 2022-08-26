using System;

namespace GrpcForUnity.DemoScenes.GeneralCommand.ModelDemo
{
    public enum WeaponType
    {
        Sword,
        Wood,
        Axe,
        Arrow
    }
    
    [Serializable]
    public class WeaponInfo
    {
        public WeaponType Type;
        public string Name;
        public int Damage;
    }
}