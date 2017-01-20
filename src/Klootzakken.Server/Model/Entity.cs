using JetBrains.Annotations;

namespace Klootzakken.Server.Model
{
    public abstract class Entity
    {
        protected Entity(Entity src)
        {
            Id = src.Id;
            Name = src.Name;
        }

        protected Entity([NotNull] string id, [NotNull] string name)
        {
            Id = id;
            Name = name;
        }

        [NotNull]
        public string Id { get; }
        [NotNull]
        public string Name { get; }
    }
}