using ECS.Utilities;
using System;
using System.Collections.Generic;
using Bitfield = System.UInt64;
using EntityComponent = System.Object;

namespace ECS.Managers
{
    class EntityComponentManager
    {        
        readonly EntityWorld _world;
        readonly Dictionary<Type, Bitfield> _registeredComponentTypes = new Dictionary<Type, Bitfield>();        
        readonly Dictionary<Type, Func<EntityComponent>> _componentFactories = new Dictionary<Type, Func<EntityComponent>>();
        readonly Dictionary<Bitfield, Pool<EntityComponent>> _componentsPools = new Dictionary<Bitfield, Pool<EntityComponent>>();

        Bitfield nextBit = 1;

        public EntityComponentManager(EntityWorld world)
        {
            _world = world;
        }

        internal Bitfield GetComponentBitForType(Type type)
        {
            Bitfield componentType;
            if (!_registeredComponentTypes.TryGetValue(type, out componentType))
            {
                componentType = nextBit;
                nextBit <<= 1;
                _registeredComponentTypes.Add(type, componentType);
            }
            return componentType;
        }

        internal Bitfield GetComponentBitForType<T>() where T : class => GetComponentBitForType(typeof(T));

        internal Bitfield GetComponentBitsForTypes(Type[] types)
        {
            Bitfield componentBits = 0;
            foreach (Type type in types)
                componentBits |= GetComponentBitForType(type);
            return componentBits;
        }

        internal T CreateComponentFromPool<T>() where T : class
        {
            Type type = typeof(T);
            Bitfield bit = GetComponentBitForType(type);
            Pool<EntityComponent> componentPool;
            if (!_componentsPools.TryGetValue(bit, out componentPool))
            {
                Func<EntityComponent> factory;
                if (!_componentFactories.TryGetValue(type, out factory))
                {
                    factory = () => Activator.CreateInstance<T>();
                    _componentFactories.Add(type, factory);
                }
                componentPool = new Pool<EntityComponent>(factory, _world.Configuration.InitialComponentPoolCapacity);
                _componentsPools.Add(bit, componentPool);
            }
            return (T)componentPool.Fetch();
        }

        internal void ReleaseComponentToPool(Bitfield bit, EntityComponent component)
        {
            Pool<EntityComponent> componentPool;
            if (_componentsPools.TryGetValue(bit, out componentPool))
                componentPool.Release(component);
        }
    }
}
