using Scellecs.Morpeh;

namespace Features.FightPack.Components
{
    public struct DelayAttackComponent : IComponent
    {
        public Entity target;

        public DelayAttackComponent(Entity target)
        {
            this.target = target;
        }
    }
}
