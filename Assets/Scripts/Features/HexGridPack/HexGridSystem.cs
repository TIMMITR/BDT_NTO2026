using Features.HexGridPack.Components;
using Features.HexGridPack.Views;
using Scellecs.Morpeh;
using Unity.Mathematics;
using UnityEngine;

namespace Features.HexGridPack
{
    public sealed class HexGridSystem : IInitializer
    {
        public World World { get; set; }
        
        private const int GridRadius = 4; // todo вынести в настройки
        private const float HexRadius = 2f;

        private Stash<HexGridComponent> _hexGridStash;
        private Stash<HexTileComponent> _hexTileStash;

        private readonly Transform _gridStartPoint;
        private readonly GameObject _boardGameObject;

        public HexGridSystem(GameObject BoardGameObject)
        {
            _boardGameObject = BoardGameObject;
        }

        public void OnAwake()
        {
            _hexGridStash = World.GetStash<HexGridComponent>();
            _hexTileStash = World.GetStash<HexTileComponent>();

            CreateGrid();
        }

        private void CreateGrid()
        {
            var root = _boardGameObject.GetComponentInChildren<GridStartPointView>().transform;
            if (!root)
            {
                Debug.LogWarning("No GridStartPointView found on " + _boardGameObject.name);
                return;
            }

            var gridEntity = World.CreateEntity();
            ref var gridComp = ref _hexGridStash.Add(gridEntity);
            gridComp.root = root;
            gridComp.hexRadius = HexRadius;
            gridComp.gridRadius = GridRadius;

            for (int q = -GridRadius; q <= GridRadius; q++)
            {
                int rMin = Mathf.Max(-GridRadius, -q - GridRadius);
                int rMax = Mathf.Min(GridRadius, -q + GridRadius);

                for (int r = rMin; r <= rMax; r++)
                {
                    var tileEntity = World.CreateEntity();
                    ref var tileComp = ref _hexTileStash.Add(tileEntity);
                    tileComp.q = q;
                    tileComp.r = r;
                }
            }
        }

        public void Dispose()
        {
            _hexGridStash = null;
            _hexTileStash = null;
        }
    }
}