using System.Collections.Generic;
using Configs;
using Features.SpeedPack.Requests;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.StatePack.Requests;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.EnemyAIPack.System
{
    public class EnemyAISystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitFilter;
        
        private Event<PhaseChangedEvent> _phaseChangedEvent;

        private Request<EndEnemyTurnRequest> _endTurnRequest;
        
        private readonly EnemyConfigSo _config;

        private List<Entity> _units;
        
        private int _attackWeight;
        private int _captureWeight;
        
        private bool _isActive;

        public EnemyAISystem(EnemyConfigSo config)
        {
            _config = config;
            
            _units = new List<Entity>();
        }

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            
            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _phaseChangedEvent.publishedChanges)
                if (evt.phase == TurnPhase.Enemy && evt.type == TurnType.Start)
                    BeginPhase();

            if (!_isActive)
                return;

            if (_units.Count == 0)
            {
                Debug.Log("Все юниты уничтожены");
                _endTurnRequest.Publish(new EndEnemyTurnRequest());
                return;
            }
            
            
        }

        private void BeginPhase()
        {
            foreach (var unit in _unitFilter)
            {
                _units.Add(unit);
            }

            _attackWeight = _config.AttackWeight;
            _captureWeight = _config.CaptureWeight;
            
            _isActive = true;
        }

        public void Dispose() { }
    }
}
