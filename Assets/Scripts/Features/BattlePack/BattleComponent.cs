using Features.StoryTalkPack.Data;
using Scellecs.Morpeh;

namespace Features.BattlePack
{
    public struct BattleComponent : IComponent
    {
        public BattlePlan battlePlan;
        public StoryTalk endStoryTalk;
        public int currentBattleId; // todo возможно, стоит убрать

        public BattleComponent(BattlePlan battlePlan, StoryTalk endStoryTalk, int currentBattleId)
        {
            this.battlePlan = battlePlan;
            this.endStoryTalk = endStoryTalk;
            this.currentBattleId = currentBattleId;
        }
    }
}