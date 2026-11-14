using Scellecs.Morpeh;

namespace Features.UnitPack
{
    public sealed class UnitSpawnSystem : ISystem 
    {
        public World World { get; set; }

        private Request<UnitSpawnRequest> _request;
        public void OnAwake()
        {
            _request = World.GetRequest<UnitSpawnRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _request.Consume())
            {
                // todo тут спавн и привязка к сетке я тоже это тебе оставлю
            }
        }

        public void Dispose() { }
    }
}