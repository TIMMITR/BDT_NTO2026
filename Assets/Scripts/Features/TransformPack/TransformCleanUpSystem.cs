using Scellecs.Morpeh;

namespace Features.TransformPack
{
    public sealed class TransformCleanUpSystem : IInitializer
    {
        public World World { get; set; }
        private Filter _filter;

        public void OnAwake()
        {
            _filter = World.Filter.With<TransformComponent>().Build();
        }
        
        public void Dispose()
        {
            foreach (var entity in _filter)
            {
                World?.RemoveEntity(entity);
            }
        }
    }
}