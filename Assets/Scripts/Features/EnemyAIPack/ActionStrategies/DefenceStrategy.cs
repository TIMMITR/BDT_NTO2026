using System.Collections.Generic;
using Features.CalcPack;
using Features.GridPack;
using Features.GridPack.Data;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.ActionStrategies
{
    public class DefenceStrategy : ActionStrategy
    {
        private readonly int _radius;

        public DefenceStrategy(int radius)
        {
            _radius = radius;
        }

        public override bool TryExecute(EnemyContext context)
        {
            var unit = context.unit;
            var from = context.positionStash.Get(unit).coord;
            var baseCoord = context.positionStash.Get(context.baseFilter.First()).coord;

            if (from.DistanceTo(baseCoord) <= _radius)
                return true;

            var speed = context.speedStash.Get(unit).currentSpeed;
            if (speed <= 0)
                return false;

            List<CellCoord> resultPath = null;

            foreach (var cell in HexMath.GetRing(baseCoord, _radius))
            {
                if (!GridData.IsFreeCell(context.grid.Comp, context.cellTileStash, cell))
                    continue;

                var path = context.BuildPath(from, cell, speed);

                if (path != null && (resultPath == null || path.Count < resultPath.Count))
                    resultPath = path;
            }

            if (resultPath == null)
                return false;

            context.moveRequest.Publish(new MoveUnitRequest(unit, resultPath), allowNextFrame: true);
            return true;
        }
    }
}