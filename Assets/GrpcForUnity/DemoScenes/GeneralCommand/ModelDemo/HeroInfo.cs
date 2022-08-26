using System;

namespace GrpcForUnity.DemoScenes.GeneralCommand.ModelDemo
{
    public enum HeroType
    {
        Knight,
        Magician,
        Elf,
        Monster,
        Zombie
    }
    
    [Serializable]
    public class HeroInfo
    {
        public HeroType Type;
        public string Name;
        public int Hp;
    }
}