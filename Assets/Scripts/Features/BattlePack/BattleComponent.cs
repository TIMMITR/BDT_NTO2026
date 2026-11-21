using Scellecs.Morpeh;

namespace Features.BattlePack
{
    public struct BattleComponent : IComponent
    {
        public BattlePlan battlePlan;
        public int currentBattleId; //todo возможно, стоит убрать
    }
}