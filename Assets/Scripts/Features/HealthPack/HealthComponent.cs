using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public struct HealthComponent : IComponent
    {
        public int currentHealth;
        public int maxHealth;

        public HealthComponent(int value)
        {
            maxHealth = value;
            currentHealth = maxHealth;
        }
    }
}
