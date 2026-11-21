using Features.PlayerPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PlayerPack.Views
{
    public class PlayerStateChangeView : MonoBehaviour
    {
        public void OnChangeCameraStateClicked()
        {
            if (World.Default == null)
            {
                Debug.LogError("PlayerStateChangeView.OnChangeCameraStateClicked() World.Default == null");
                return;
            }
            
            World.Default.GetRequest<PlayerStateChangeRequest>().Publish(new PlayerStateChangeRequest());
        }
    }
}