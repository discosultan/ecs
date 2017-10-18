using System;
using System.Collections.Generic;
using Bitfield = System.UInt64;
using EntityComponent = System.Object;

namespace ECS
{
    // TODO: Add layering support?
    public sealed class Entity : IDisposable, IEquatable<Entity>
    {
        // For 64 bit types, the actual shift count is 63.
        //     32                                      31
        // Ref: https://msdn.microsoft.com/en-us/library/a1sway8w.aspx
        const int BitfieldLength = 63;

        bool _enabled = true;

        internal int Id;
        internal Bitfield ComponentBits;        
        internal Dictionary<Bitfield, EntityComponent> Components = new Dictionary<Bitfield, EntityComponent>();

        internal Entity() { }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    World.SetAllContextsDirty();
                }
            }
        }

        public EntityWorld World { get; internal set; }

        public Entity AddComponent<T>(T component) where T : class
        {
            World.EntityManager.AddComponentToEntity(typeof(T), component, this);
            return this;
        }
        public Entity AddComponent(object component)
        {            
            World.EntityManager.AddComponentToEntity(component, this);
            return this;
        }

        public T GetComponent<T>() where T : class => World.EntityManager.GetComponentFromEntity<T>(this);
        public Dictionary<Bitfield, EntityComponent>.ValueCollection GetComponents() => Components.Values;

        public T RemoveComponent<T>() where T : class => World.EntityManager.RemoveComponentFromEntity<T>(this);

        public void Dispose() => World.EntityManager.ReleaseEntityToPool(this);

        internal void GetComponentsForComponentBits(Bitfield systemComponentBits, int numberOfSystemComponents, EntityComponent[] componentsContainer)
        {
            int componentCounter = 0;
            for (Bitfield bit = 1; bit <= 1 << BitfieldLength - 1; bit <<= 1)
                if ((bit & systemComponentBits) > 0)
                {
                    componentsContainer[componentCounter++] = Components[bit];
                    if (componentCounter >= numberOfSystemComponents)
                        break;
                }
        }

        public bool Equals(Entity other) => Id == other?.Id && World.Equals(other.World);
        public override bool Equals(object obj) => Equals(obj as Entity);
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
        public override int GetHashCode() => Id;
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
    }

    public class EntityState
    {
        public bool Enabled { get; set; }
        public object[] Components { get; set; }
    }
}
