namespace Features.EnemyAIPack.ActionStrategies
{
    public abstract class ActionStrategy
    {
        public float weight;
        
        private readonly float _decrease;
        
        public void Spend () => weight -= _decrease;

        protected ActionStrategy(float weight, float decrease)
        {
            this.weight = weight;
            _decrease = decrease;
        }

        public abstract bool TryExecute(EnemyContext context);
    }
}
