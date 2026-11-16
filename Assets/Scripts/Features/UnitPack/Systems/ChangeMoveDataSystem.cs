using System.Collections.Generic;
using Features.GridPack.Components;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;
using Scenes.Game;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public class ChangeMoveDataSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _dataFilter;
        
        private Stash<MoveDataComponent> _dataStash;
        
        private Request<ChangeMoveDataRequest> _changeDataRequest;
        
        private readonly List<MoveData> _moveDataList;
        
        public ChangeMoveDataSystem(List<MoveData> moveDataList)
        {
            _moveDataList = moveDataList;
            
            Debug.Log(_moveDataList.Count);
        }
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _dataFilter = World.Filter.With<MoveDataComponent>().Build();
            
            _dataStash = World.GetStash<MoveDataComponent>();
            
            _changeDataRequest = World.GetRequest<ChangeMoveDataRequest>();
            
            var gridEntity = _gridFilter.First();
            
            _dataStash.Add(gridEntity, new MoveDataComponent{moveData = _moveDataList[0]});
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _changeDataRequest.Consume())
            {
                var dataEntity = _dataFilter.First();
                ref var dataComp = ref _dataStash.Get(dataEntity);

                var newDataIndex = request.dataIndex;

                if (newDataIndex < 0 || newDataIndex >= _moveDataList.Count)
                {
                    Debug.Log("Wrong index");
                    continue;
                }
                
                var newMoveData = _moveDataList[newDataIndex];
                
                dataComp.moveData = newMoveData;
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}
