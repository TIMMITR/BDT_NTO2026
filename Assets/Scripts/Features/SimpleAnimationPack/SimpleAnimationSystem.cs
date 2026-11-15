using System;
using Configs;
using Global;
using PrimeTween;
using Scellecs.Morpeh;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.SimpleAnimationPack
{
    public sealed class SimpleAnimationSystem : ISystem
    {
        public World World { get; set; }

        private readonly WorldSettingsSo _settings;
        private Filter _filter;
        private Stash<SimpleAnimationComponent> _simpleAnimationStash;
        private Stash<SimpleAnimationActiveTag> _activeStash;
        
        public SimpleAnimationSystem(WorldSettingsSo settings)
        {
            _settings = settings;
        }

        public void OnAwake()
        {
            _filter = World.Filter.With<SimpleAnimationComponent>().Build();
            _simpleAnimationStash = World.GetStash<SimpleAnimationComponent>();
            _activeStash = World.GetStash<SimpleAnimationActiveTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var component = ref _simpleAnimationStash.Get(entity);

                if (component.transform == null)
                {
                    if (component.tween.isAlive)
                    {
                        component.tween.Stop();
                    }
                    continue;
                }

                bool isFirstStart = false;
                if (!component.isInitialized)
                {
                    Initialize(ref component);
                    component.isInitialized = true;
                    isFirstStart = true;
                }

                bool shouldBeActive = !_activeStash.Has(entity);

                if (shouldBeActive)
                {
                    if (!component.tween.isAlive)
                    {
                        component.tween = StartTween(ref component, isFirstStart);
                    }
                }
                else
                {
                    if (component.tween.isAlive)
                    {
                        component.tween.Stop();
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var entity in _filter)
            {
                ref var component = ref _simpleAnimationStash.Get(entity);
                if (component.tween.isAlive)
                {
                    component.tween.Stop();
                }
            }
        }

        private void Initialize(ref SimpleAnimationComponent component)
        {
            component.basePositionY = component.transform.localPosition.y;
            

            switch (component.animationType)
            {
                case AnimationType.Rotation:
                    component.speed *= Random.Range(_settings.rotationSpeedModifier.x, _settings.rotationSpeedModifier.y);

                    if (_settings.randomRotationDirection && Random.value > 0.5f)
                        component.speed = -component.speed;

                    if (_settings.randomInitialRotation)
                        component.transform.Rotate(0f, Random.Range(0f, 360f), 0f, Space.Self);
                    break;

                case AnimationType.Bobbing:
                    component.amplitude += Random.Range(_settings.bobbingAmplitudeBonus.x, _settings.bobbingAmplitudeBonus.y);
                    component.speed *= Random.Range(_settings.bobbingSpeedModifier.x, _settings.bobbingSpeedModifier.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Tween StartTween(ref SimpleAnimationComponent component, bool isFirstStart)
        {
            float absSpeed = Mathf.Abs(component.speed);
            float duration = absSpeed > 0.0001f ? 1f / absSpeed : 1f;

            switch (component.animationType)
            {
                case AnimationType.Rotation:
                    float directionSign = Mathf.Sign(component.speed);
                    return Tween.LocalEulerAngles(
                        component.transform,
                        startValue: component.transform.localEulerAngles,
                        endValue: component.transform.localEulerAngles + new Vector3(0f, 360f * directionSign, 0f),
                        duration: duration,
                        ease: component.ease,
                        cycles: -1,
                        cycleMode: CycleMode.Restart
                    );

                case AnimationType.Bobbing:
                    var tween = Tween.LocalPositionY(
                        component.transform,
                        startValue: component.basePositionY,
                        endValue: component.basePositionY + component.amplitude,
                        duration: duration,
                        ease: component.ease,
                        cycles: -1,
                        cycleMode: CycleMode.Yoyo
                    );

                    if (isFirstStart && _settings != null && _settings.randomizeBobbingPhase)
                    {
                        tween.progress = Random.value;
                    }

                    return tween;

                default:
                    return default;
            }
        }
    }
}