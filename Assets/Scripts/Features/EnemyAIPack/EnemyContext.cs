using System.Collections.Generic;
using Features.ControlPointPack.Components;
using Features.FightPack.Components;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Features.SpeedPack;
using Features.SpellPack.Components;
using Features.StatePack.Components;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Global;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack
{
    public class EnemyContext
    {
        public Entity unit;
        
        public Filter unitFilter;
        public Filter controlPointFilter;
        public Filter baseFilter;

        public Singletone<GridComponent> grid;

        public Stash<CellPositionComponent> positionStash;
        public Stash<CellTileComponent> cellTileStash;
        public Stash<ControlPointComponent> controlPointStash;
        public Stash<SpeedComponent> speedStash;
        public Stash<UnitComponent> unitStash;
        public Stash<SpellDataComponent> spellStash;
        public Stash<AttackDesireComponent> attackDesireStash;

        public Request<MoveUnitRequest> moveRequest;

        public List<CellCoord> BuildPath(CellCoord from, CellCoord to, int speed)
        {
            var ready = PathFinder.GetPath(grid.Comp, from, to, cellTileStash);

            if (!ready.path.ContainsKey(to))
                return null;

            var path = PathFinder.ReconstructPath(ready, from, to);

            if (path.Count <= 1)
                return null;

            path.Insert(0, from);

            while (path.Count - 1 > speed)
                path.RemoveAt(path.Count - 1);

            return path;
        }
    }
}