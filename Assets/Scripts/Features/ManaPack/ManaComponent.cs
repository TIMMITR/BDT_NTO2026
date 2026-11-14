using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public struct ManaComponent : IComponent
    {
        public int currentMana;
        public int maxMana;

        public ManaComponent(int value)
        {
            maxMana = value;
            currentMana = maxMana;
        }
    }
}