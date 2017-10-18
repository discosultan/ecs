using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Utilities;
using Bitfield = System.UInt64;
using EntityComponent = System.Object;

namespace ECS.Managers
{
    class EntityManager
    {
        readonly EntityWorld _world;
        readonly Pool<Entity> _entityPool;

        int _entityIdSequence = 1;

        internal EntityManager(EntityWorld world)
        {
            _world = world;
            _entityPool = new Pool<Entity>(() => new Entity(), world.Configuration.InitialEntityPoolCapacity);
        }

        internal Dictionary<int, Entity> Entities { get; } = new Dictionary<int, Entity>();

        internal Entity FetchEntityFromPool()
        {
            Entity entity = _entityPool.Fetch();            
            entity.World = _world;
            entity.Enabled = true;
            entity.Id = _entityIdSequence++;
            Entities.Add(entity.Id, entity);
            return entity;
        }

        internal void ReleaseEntityToPool(Entity entity)
        {
            entity.World = null;
            foreach (KeyValuePair<Bitfield, EntityComponent> componentEntry in entity.Components)
                _world.EntityComponentManager.ReleaseComponentToPool(componentEntry.Key, componentEntry.Value);
            entity.Components.Clear();
            Entities.Remove(entity.Id);
            _entityPool.Release(entity);            
        }

        internal void AddComponentToEntity(object component, Entity entity)
        {
            Guard.NotNull(component, nameof(component));
            AddComponentToEntity(component.GetType(), component, entity);
        }

        internal void AddComponentToEntity(Type type, object component, Entity entity)
        {
            Guard.NotNull(component, nameof(component));
            Bitfield componentBit = _world.EntityComponentManager.GetComponentBitForType(type);

            Guard.NotContainedIn(componentBit, entity.Components);

            entity.Components.Add(componentBit, component);
            entity.ComponentBits |= componentBit;
            _world.SetAllContextsDirty();
        }

        internal T GetComponentFromEntity<T>(Entity entity) where T : class
        {
            Bitfield componentBit = _world.EntityComponentManager.GetComponentBitForType<T>();
            entity.Components.TryGetValue(componentBit, out EntityComponent component);
            return (T)component;
        }

        internal T RemoveComponentFromEntity<T>(Entity entity) where T : class
        {
            Type type = typeof(T);
            Bitfield componentBit = _world.EntityComponentManager.GetComponentBitForType(type);

            if (entity.Components.TryGetValue(componentBit, out EntityComponent component))
                if (entity.Components.Remove(componentBit))
                {
                    entity.ComponentBits &= ~componentBit;
                    _world.SetAllContextsDirty();
                }

            _world.EntityComponentManager.ReleaseComponentToPool(componentBit, component);
            return (T)component;
        }

        internal void Clear()
        {
            Entity[] entitiesToRemove = Entities.Values.ToArray();
            foreach (Entity entity in entitiesToRemove)
                entity.Dispose();
        }

        internal void GetState(EntityWorldState state)
        {
            state.Entities = Entities.Values.Select(x => new EntityState
            {
                Enabled = x.Enabled,
                Components = x.Components.Values.Select(y => y.CloneJson()).ToArray()
            }).ToArray();
        }

        internal void RestoreState(EntityWorldState state)
        {
            Clear();
            foreach (var entityState in state.Entities)
            {
                Entity entity = FetchEntityFromPool();
                entity.Enabled = entityState.Enabled;
                foreach (var component in entityState.Components)
                    entity.AddComponent(component);
            }
        }
    }
}
