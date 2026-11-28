namespace Features.EnemyAIPack.ActionStrategies
{
    public abstract class ActionStrategy
    { 
        public abstract bool TryExecute(EnemyContext context);
    }
}
