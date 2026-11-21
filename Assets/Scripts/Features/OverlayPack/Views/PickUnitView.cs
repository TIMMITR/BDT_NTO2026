using Scellecs.Morpeh;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public class PickUnitView : MonoBehaviour
    {
        public int id; // номер героя 
        
        public void OnPickUnit()
        {
            if (World.Default == null) 
                return;
            
            World.Default.GetRequest<PickUnitRequest>().Publish(new PickUnitRequest
            {
                id = id
            });
        }
        
    }

    public struct PickUnitRequest : IRequestData
    {
        public int id; // я не уверен как тут выбор сделать, где-то нужно запоминать героя
    }
}