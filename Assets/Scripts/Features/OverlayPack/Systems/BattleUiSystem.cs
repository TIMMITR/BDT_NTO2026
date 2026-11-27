using Features.OverlayPack.Views;
using Scellecs.Morpeh;

namespace Features.OverlayPack.Systems
{
    public sealed class BattleUiSystem : ISystem
    {
        public World World { get; set; }

        private readonly BattleUiView _view;
        private Request<BattleUiRequest> _request;

        public BattleUiSystem(BattleUiView view)
        {
            _view = view;
        }

        public void OnAwake()
        {
            _request = World.GetRequest<BattleUiRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _request.Consume())
                _view.SetActive(request.active);
        }

        public void Dispose() { }
    }
}
