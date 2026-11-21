using Scellecs.Morpeh;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public class UnitStatsButton : MonoBehaviour
    {
        public int id; // номер героя 
        public bool activeStats; // открыт ли он
        public Vector2 position; // координаты по x для закрытого и открытого состояния
        
        public void OnUnitStats()
        {
            if (World.Default == null) 
                return;
            
            World.Default.GetRequest<UnitStatsRequest>().Publish(new UnitStatsRequest
            {
                id = id,
                activeStats =activeStats
            });
        }
        
    }

    public struct UnitStatsRequest : IRequestData
    {
        public int id;
        public bool activeStats;
    }
}