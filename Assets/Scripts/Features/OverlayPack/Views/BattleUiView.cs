using Scellecs.Morpeh;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public class BattleUiView : MonoBehaviour
    {
        public GameObject canvasUI;

        private Request<BattleUiRequest> _request;
        private void Awake()
        {
            _request = World.Default.GetRequest<BattleUiRequest>();
        }

        public void LateUpdate()
        {
            foreach (var req in _request.Consume())
            {
                canvasUI.SetActive(req.active);
            }
        }
    }

    public struct BattleUiRequest : IRequestData
    {
        public bool active;
    }
}