using System.Collections.Generic;
using Features.GridPack.Components;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;
using Scenes.Game;

namespace Features.UnitPack.Systems
{
    public class ChangeAnimationDataSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _animationDataFilter;
        
        private Stash<MoveAnimationDataComponent> _animationDataStash;
        
        private Request<ChangeAnimationDataRequest> _changeAnimationRequest;
        
        private readonly List<AnimationData> _animationDataList;
        
        public ChangeAnimationDataSystem(List<AnimationData> animationDataList)
        {
            _animationDataList = animationDataList;
        }
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _animationDataFilter = World.Filter.With<MoveAnimationDataComponent>().Build();
            
            _animationDataStash = World.GetStash<MoveAnimationDataComponent>();
            
            _changeAnimationRequest = World.GetRequest<ChangeAnimationDataRequest>();
            
            var gridEntity = _gridFilter.First();
            
            _animationDataStash.Add(gridEntity, new MoveAnimationDataComponent{animationData = _animationDataList[0]});
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _changeAnimationRequest.Consume())
            {
                var animationEntity = _animationDataFilter.First();
                ref var animationDataComp = ref _animationDataStash.Get(animationEntity);

                var dataIndex = request.dataIndex;

                if (dataIndex < 0 || dataIndex >= _animationDataList.Count)
                {
                    continue;
                }
                
                animationDataComp.animationData = _animationDataList[dataIndex];
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
