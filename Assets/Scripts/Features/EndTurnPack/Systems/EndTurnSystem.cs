using Features.EndTurnPack.Requests;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.EndTurnPack.Systems
{
    public class EndTurnSystem : ISystem
    {
        public World World { get; set; }
        
        private Stash<TurnEndedTag> _moveEndedTag;
        
        private Request<EndTurnRequest> _endTurnRequest;

        public void OnAwake()
        {
            _moveEndedTag = World.GetStash<TurnEndedTag>();
            
            _endTurnRequest = World.GetRequest<EndTurnRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _endTurnRequest.Consume())
            {
                _moveEndedTag.Add(World.CreateEntity());
            }
        }
        
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
