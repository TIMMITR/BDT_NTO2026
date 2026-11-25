using System.Collections.Generic;
using Features.UnitPack.Events;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;
using SelectUnitRequest = Features.CellSelectionPack.Requests.SelectUnitRequest;

namespace Features.OverlayPack.Views
{
    public class PickUnitsPanelView : MonoBehaviour
    {
        [SerializeField] private List<PickUnitView> pickUnitViews;
        
        private Event<RegisterUnitEvent> _registerUnitEvent;
        private Request<SelectUnitRequest> _selectUnitRequest;

        private int _nextButtonIndex;

        private void Awake()
        {
            _registerUnitEvent = World.Default.GetEvent<RegisterUnitEvent>();
            _selectUnitRequest = World.Default.GetRequest<SelectUnitRequest>();

            _registerUnitEvent.Subscribe(changes =>
            {
                for (var i = 0; i < changes.length && _nextButtonIndex < pickUnitViews.Count; i++)
                {
                    var unit = changes[i].unit;
                    var button = pickUnitViews[_nextButtonIndex++].pickUnitButton;

                    button.onClick.AddListener(() =>
                        _selectUnitRequest.Publish(new SelectUnitRequest
                        {
                            unit = unit
                        }));
                }
            });
        }

    }   
}