using Features.InputPack;
using Global;
using Scellecs.Morpeh;

namespace Features.PlayerPack.Systems
{
    public class PlayerSpawnSystem : IInitializer
    {
        public World World { get; set; }
        
        public void OnAwake()
        {
            var player = World.CreateEntity();
            World.GetStash<PlayerTag>().Add(player);
            World.GetStash<InputComponent>().Add(player);
        }
        
        public void Dispose() { }
    }
}
