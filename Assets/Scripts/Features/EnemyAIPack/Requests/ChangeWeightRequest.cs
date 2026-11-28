using Scellecs.Morpeh;

namespace Features.EnemyAIPack.Requests
{
    public struct ChangeWeightRequest : IRequestData 
    {
        public EnemyAIAction type;
        public ChangeWeightType changeWeightType;

        public ChangeWeightRequest(EnemyAIAction type, ChangeWeightType changeWeightType)
        {
            this.type = type;
            this.changeWeightType = changeWeightType;
        }
    }

    public enum ChangeWeightType
    {
        Increase,
        Decrease
    }
}
