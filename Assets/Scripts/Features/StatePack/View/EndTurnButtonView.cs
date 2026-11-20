using Features.StatePack.Requests;
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
            
            World.Default.GetRequest<EndPlayerTurnRequest>().Publish(new EndPlayerTurnRequest());
        }
    }
}
