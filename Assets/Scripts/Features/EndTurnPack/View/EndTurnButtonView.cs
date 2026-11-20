using Features.EndTurnPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.EndTurnPack.View
{
    public class EndTurnButtonView : MonoBehaviour
    {
        public void OnEndTurnClicked()
        {
            if (World.Default == null) 
                return;
            
            World.Default.GetRequest<EndTurnRequest>().Publish(new EndTurnRequest());
        }
    }
}
