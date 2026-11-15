using PrimeTween;
using Scellecs.Morpeh.Providers;

namespace Features.SimpleAnimationPack
{
    public class SimpleAnimationAuthoring : MonoProvider<SimpleAnimationComponent>
    {
        public AnimationType animationType;
        public float speed = 1.0f;
        public float amplitude = 0.5f;
        public Ease ease = Ease.OutSine; 
        
        private void Start()
        {
            ref var saComponent = ref GetData();
            saComponent.animationType = animationType;
            saComponent.speed = speed;
            saComponent.amplitude = amplitude;
            saComponent.ease = ease;
            saComponent.transform = transform;
            saComponent.isInitialized = false;
        }
    }
}