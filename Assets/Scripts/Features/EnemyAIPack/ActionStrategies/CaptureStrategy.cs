using System.Collections.Generic;
using Features.CalcPack;
using Features.GridPack;
using Features.GridPack.Data;
using Features.UnitPack.Requests;

namespace Features.EnemyAIPack.ActionStrategies
{
    public class CaptureStrategy : ActionStrategy
    {
        public CaptureStrategy(float weight, float decrease) : base(weight, decrease) { }

        public override bool TryExecute(EnemyContext context)
        {
            var unit = context.unit;
            var from = context.positionStash.Get(unit).coord;
            var speed = context.speedStash.Get(unit).currentSpeed;

            if (speed <= 0)
                return false;

            List<CellCoord> resultPath = null;

            foreach (var controlPoint in context.controlPointFilter)
            {
                var coord = context.controlPointStash.Get(controlPoint).cellCoord;

                foreach (var neighbour in HexMath.GetNeighbours(coord))
                {
                    if (!GridData.IsFreeCell(context.grid.Comp, context.cellTileStash, neighbour))
                        continue;

                    var path = context.BuildPath(from, neighbour, speed);

                    if (path != null && (resultPath == null || path.Count < resultPath.Count))
                        resultPath = path;
                }
            }

            if (resultPath == null)
                return false;

            context.moveRequest.Publish(new MoveUnitRequest(unit, resultPath), allowNextFrame: true);
            return true;
        }
    }
}