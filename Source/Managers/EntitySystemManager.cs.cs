using System;
using System.Collections.Generic;
using ECS.Utilities;

namespace ECS.Managers
{
    class EntitySystemManager
    {
        readonly EntityWorld _world;

        int _systemIdSequence = 1;

        internal EntitySystemManager(EntityWorld world)
        {
            _world = world;
        }

        internal Dictionary<Type, EntitySystem> Systems { get; } = new Dictionary<Type, EntitySystem>();
        internal List<EntitySystem> SystemsForUpdate { get; } = new List<EntitySystem>();
        internal List<EntitySystem> SystemsForDraw { get; } = new List<EntitySystem>();

        internal EntityWorld AddSystem<T>(T system) where T : EntitySystem
        {
            Type type = typeof(T);

            Guard.NotNull(system, nameof(system));
            Guard.True(system.World == null);
            Guard.NotContainedIn(type, Systems);

            system.World = _world;
            system.Id = _systemIdSequence++;
            system.ComponentBits = _world.EntityComponentManager.GetComponentBitsForTypes(system.ComponentTypes);
            Systems.Add(type, system);
            if (system.GameLoopType == GameLoopType.Update)
            {
                SystemsForUpdate.Add( system);
                _world.UpdateContext.Dirty = true;
            }
            else
            {
                SystemsForDraw.Add(system);
                _world.DrawContext.Dirty = true;
            }
            return _world;
        }

        internal T RemoveSystem<T>() where T : EntitySystem
        {
            Type type = typeof(T);
            if (Systems.TryGetValue(type, out EntitySystem system))
            {
                Systems.Remove(type);
                if (!SystemsForUpdate.Remove(system))
                    SystemsForDraw.Remove(system);
            }
            return (T)system;
        }
    }
}
