using Scellecs.Morpeh;

namespace Global
{
    public class Singletone<T> where T : struct, IComponent
    {
        private readonly Filter _filter;
        private readonly Stash<T> _stash;

        public Singletone(World world) // todo возможно стоит проверять на != null и брать просто World.
        {
            _filter = world.Filter.With<T>().Build();
            _stash = world.GetStash<T>();
        }

        public Entity Entity => _filter.First();
        public ref T Comp => ref _stash.Get(_filter.First());
    }
}
