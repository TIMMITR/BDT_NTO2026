using Features.BattlePack;
using Features.GridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ControlPointPack.Requests
{
    public struct SpawnControlPointRequest : IRequestData
    {
        public ControlPointConfig config;
        public CellCoord cellCoord;
        
        public SpawnControlPointRequest(ControlPointConfig config, CellCoord cellCoord)
        {
            this.config = config;
            this.cellCoord = cellCoord;
        }
    }
}
