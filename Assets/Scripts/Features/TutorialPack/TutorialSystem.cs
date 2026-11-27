using System;
using System.Collections.Generic;
using Configs;
using Features.CellTrackPack.Events;
using Features.FightPack.Components;
using Features.HealthPack;
using Features.SpellPack.Components;
using Features.StatePack.Components;
using Features.StatePack.Events;
using Features.StoryTalkPack.Data;
using Features.StoryTalkPack.Events;
using Features.StoryTalkPack.Requests;
using Features.TypingPack.Components;
using Features.TypingPack.Requests;
using Features.UnitPack.Components;
using Features.UnitPack.Events;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.TutorialPack
{
    public enum TutorialSignal
    {
        None,
        UnitMoved,
        SpellSelected,
        CellHovered,
        SpellUsed,
        EnemyHit,
        PlayerTurnEnded,
        BaseDestroyed 
    }

    [Serializable]
    public class TutorialStep
    {
        public StoryTalk talk;
        public TutorialSignal stopSignal = TutorialSignal.None;
        public bool freezeTime = true;
    }
    
    public sealed class TutorialSystem : ISystem
    {
        public World World { get; set; }

        private readonly PlayerInputActions _inputActions;
        private readonly List<TutorialStep> _steps;

        private int _index = -1;
        private bool _isStarted;
        private bool _isFinished;
        private bool _isTalkPlaying;
        private bool _frozen;
        private float _savedTimeScale = 1f;
        private int _anotherTalkDepth;
        private int _skipFrames = 2;

        private bool _spellSelectedPrev;
        private bool _baseDeadPrev;

        private Filter _spellFilter;
        private Filter _attackFilter;
        private Filter _baseFilter;
        private Filter _typingFilter;
        private Filter _phaseFilter;

        private Stash<UnitComponent> _unitStash;
        private Stash<TypingComponent> _typingStash;
        private Stash<TurnPhaseComponent> _phaseStash;
        
        private Request<StoryTalkShowRequest> _talkShowRequest;
        private Request<TypingSkipRequest> _typingSkipRequest;
        
        private Event<UnitMovedEvent> _movedEvent;
        private Event<CellChooseChangedEvent> _cellEvent;
        private Event<HealthChangedEvent> _healthEvent;
        private Event<PhaseChangedEvent> _phaseEvent;
        private Event<StoryTalkStartEvent> _talkStartEvent;
        private Event<StoryTalkFinishEvent> _talkFinishEvent;

        public TutorialSystem(PlayerInputActions inputActions, List<TutorialStep> steps)
        {
            _inputActions = inputActions;
            _steps = steps;
        }

        public void OnAwake()
        {
            _movedEvent = World.GetEvent<UnitMovedEvent>();
            _cellEvent = World.GetEvent<CellChooseChangedEvent>();
            _healthEvent = World.GetEvent<HealthChangedEvent>();
            _phaseEvent = World.GetEvent<PhaseChangedEvent>();
            _talkStartEvent = World.GetEvent<StoryTalkStartEvent>();
            _talkFinishEvent = World.GetEvent<StoryTalkFinishEvent>();

            _talkShowRequest = World.GetRequest<StoryTalkShowRequest>();
            _typingSkipRequest = World.GetRequest<TypingSkipRequest>();

            _spellFilter = World.Filter.With<SpellDataComponent>().With<UnitComponent>().Build();
            _attackFilter = World.Filter.With<AttackDesireComponent>().With<UnitComponent>().Build();
            _baseFilter = World.Filter.With<BaseTag>().With<DeadTag>().Build();
            _typingFilter = World.Filter.With<TypingComponent>().Build();
            _phaseFilter = World.Filter.With<TurnPhaseComponent>().Build();

            _unitStash = World.GetStash<UnitComponent>();
            _typingStash = World.GetStash<TypingComponent>();
            _phaseStash = World.GetStash<TurnPhaseComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            var spellSelectedNow = PlayerSpellSelected();
            var baseDeadNow = !_baseFilter.IsEmpty();

            var sawTalkFinish = false;
            
            foreach (var _ in _talkStartEvent.publishedChanges)
                _anotherTalkDepth++;
            
            foreach (var _ in _talkFinishEvent.publishedChanges)
            {
                _anotherTalkDepth--;
                sawTalkFinish = true;
            }

            if (_isFinished)
                return;

            if (!_isStarted)
            {
                if (_skipFrames > 0)
                {
                    _skipFrames--;  
                }
                else if (_anotherTalkDepth <= 0)  // todo возможно стоит кидать ошибку при <0
                { 
                    _isStarted = true;
                    NextStep();
                }
            }
            else if (_isTalkPlaying)
            {
                TickTalk(sawTalkFinish);
            }
            else if (SignalFired(_steps[_index].stopSignal, spellSelectedNow, baseDeadNow))
            {
                NextStep();
            }

            _spellSelectedPrev = spellSelectedNow;
            _baseDeadPrev = baseDeadNow;
        }

        private void NextStep()
        {
            while (true)
            {
                _index++;

                if (_index >= _steps.Count)
                {
                    Finish();
                    return;
                }

                var step = _steps[_index];

                if (step.talk != null)
                {
                    StartTalk(step);
                    return;
                }
                
                if (step.stopSignal != TutorialSignal.None)
                    return;
            }
        }

        private void StartTalk(TutorialStep step)
        {
            _isTalkPlaying = true;
            _inputActions.Gameplay.Disable();

            if (step.freezeTime)
            {
                _savedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                _frozen = true;
            }

            _talkShowRequest.Publish(new StoryTalkShowRequest(step.talk));
        }

        private void TickTalk(bool sawFinish)
        {
            if (_frozen)
            {
                var typing = _typingStash.Get(_typingFilter.First());
                if (typing.active && typing.isTyping)
                    _typingSkipRequest.Publish(new TypingSkipRequest());
            }

            if (!sawFinish)
                return;

            _isTalkPlaying = false;
            _inputActions.Gameplay.Enable();

            if (_frozen)
            {
                Time.timeScale = _savedTimeScale;
                _frozen = false;
            }

            if (_steps[_index].stopSignal == TutorialSignal.None)
                NextStep();
        }

        private void Finish()
        {
            _isFinished = true;
            
            Debug.Log("Step finished");
        }

        private bool SignalFired(TutorialSignal signal, bool spellSelectedNow, bool baseDeadNow)
        {
            switch (signal)
            {
                case TutorialSignal.UnitMoved:
                    foreach (var _ in _movedEvent.publishedChanges)
                        return true;
                    
                    return false;

                case TutorialSignal.CellHovered:
                    foreach (var _ in _cellEvent.publishedChanges)
                        return true;
                    
                    return false;

                case TutorialSignal.SpellSelected:
                    return spellSelectedNow && !_spellSelectedPrev;

                case TutorialSignal.SpellUsed:
                    foreach (var unit in _attackFilter)
                        if (_unitStash.Get(unit).teamSide == TeamSide.Player)
                            return true;
                    
                    return false;

                case TutorialSignal.EnemyHit:
                    foreach (var e in _healthEvent.publishedChanges)
                        if (e.delta < 0 && _unitStash.Has(e.entity) && _unitStash.Get(e.entity).teamSide == TeamSide.Enemy)
                            return true;
                    
                    return false;

                case TutorialSignal.PlayerTurnEnded:
                    foreach (var e in _phaseEvent.publishedChanges)
                        if (e.phase == TurnPhase.Player && e.type == TurnType.End)
                            return true;
                    
                    return false;

                case TutorialSignal.BaseDestroyed:
                    return baseDeadNow && !_baseDeadPrev;

                default:
                    return false;
            }
        }

        private bool PlayerSpellSelected()
        {
            foreach (var unit in _spellFilter)
                if (_unitStash.Get(unit).teamSide == TeamSide.Player)
                    return true;
            
            return false;
        }

        private bool IsPlayerPhase()
        {
            if (_phaseFilter.IsEmpty()) 
                return false;
            
            return _phaseStash.Get(_phaseFilter.First()).phase == TurnPhase.Player;
        }

        public void Dispose()
        {
            if (_frozen) 
                Time.timeScale = _savedTimeScale;
            
            _inputActions.Gameplay.Enable();
        }
    }
}