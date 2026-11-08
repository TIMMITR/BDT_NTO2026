using Scellecs.Morpeh;
using UnityEngine;

namespace Scenes.MainMenu
{
    public class MainMenuStartUp : MonoBehaviour
    {
        [SerializeField] private GlobalContext globalContext;
        [SerializeField] private MainMenuContext context;

        private World _world;
        private SystemsGroup _updateGroup;
        private SystemsGroup _fixedGroup;

        private void Awake()
        {
            _world = World.Default;

            _updateGroup = _world.CreateSystemsGroup();
            _updateGroup.Initialize();
            _world.AddSystemsGroup(order: 0, _updateGroup);

            _fixedGroup = _world.CreateSystemsGroup();
            _fixedGroup.Initialize();
        }

        private void FixedUpdate()
        {
            _fixedGroup?.Update(Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            if (_updateGroup != null)
            {
                _world?.RemoveSystemsGroup(_updateGroup);
                _updateGroup.Dispose();
                _updateGroup = null;
            }

            if (_fixedGroup != null)
            {
                _fixedGroup.Dispose();
                _fixedGroup = null;
            }
        }
    }
}