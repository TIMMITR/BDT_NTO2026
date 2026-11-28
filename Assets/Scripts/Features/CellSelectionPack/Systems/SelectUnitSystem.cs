using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.CellSelectionPack.Tags;
using Global;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Systems
{
    public sealed class SelectUnitSystem : ISystem
    {
        public World World { get; set; }

        private Singletone<PlayerSelectionTag> _selection;

        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<PlannedPathComponent> _plannedPathStash;
        private Stash<PlannedAttackComponent> _plannedAttackStash;
        private Stash<PlannedRangeComponent> _plannedRangeStash;

        private Request<SelectUnitRequest> _selectRequest;
        private Request<DeselectUnitRequest> _deselectRequest;
        private Request<PlanCalcRequest> _planCalcRequest;

        public void OnAwake()
        {
            _selection = new Singletone<PlayerSelectionTag>(World);

            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _plannedPathStash = World.GetStash<PlannedPathComponent>();
            _plannedAttackStash = World.GetStash<PlannedAttackComponent>();
            _plannedRangeStash = World.GetStash<PlannedRangeComponent>();

            _selectRequest = World.GetRequest<SelectUnitRequest>();
            _deselectRequest = World.GetRequest<DeselectUnitRequest>();
            _planCalcRequest = World.GetRequest<PlanCalcRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            var selection = _selection.Entity;
            var changed = false;

            foreach (var deselectReq in _deselectRequest.Consume())
            {
                _selectedUnitStash.Remove(selection);
                changed = true;
            }
            // todo сделать только в свой ход
            foreach (var selectReq in _selectRequest.Consume())
            {
                _selectedUnitStash.Set(selection, new SelectedUnitComponent(selectReq.unit));
                changed = true;
            }

            if (!changed)
                return;

            _plannedPathStash.Remove(selection);
            _plannedAttackStash.Remove(selection);
            _plannedRangeStash.Remove(selection);
            
            _planCalcRequest.Publish(new PlanCalcRequest());
        }

        public void Dispose() { }
    }
}