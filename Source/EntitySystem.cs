using System;
using Bitfield = System.UInt64;
using EntityComponent = System.Object;

namespace ECS
{
    public enum GameLoopType
    {
        Update,
        Draw
    }

    public abstract class EntitySystem : IEquatable<EntitySystem>
    {
        internal Bitfield ComponentBits;
        internal int Id;

        protected EntitySystem(Type[] componentTypes, GameLoopType loopType = GameLoopType.Update)
        {
            ComponentTypes = componentTypes;
            GameLoopType = loopType;
        }

        public bool Enabled { get; set; } = true;
        public EntityWorld World { get; internal set; }   
        public Type[] ComponentTypes { get; }        

        internal GameLoopType GameLoopType { get; }
        internal int NumberOfBits => ComponentTypes.Length;
        protected internal abstract void Process(EntityComponent[] components);

        public bool Equals(EntitySystem other) => Id == other?.Id;
        public override bool Equals(object obj) => Equals(obj as EntitySystem);
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
        public override int GetHashCode() => Id;
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
    }

    public abstract class EntitySystem<T> : EntitySystem where T : class
    {        
        protected EntitySystem(GameLoopType loopType = GameLoopType.Update) : base(new[] { typeof(T) }, loopType)
        { }
        
        protected abstract void Process(T component);

        protected internal override void Process(EntityComponent[] components) => Process((T) components[0]);        
    }

    public abstract class EntitySystem<T1, T2> : EntitySystem where T1 : class where T2 : class
    {
        protected EntitySystem(GameLoopType loopType = GameLoopType.Update) : base(new[] { typeof(T1), typeof(T2) }, loopType)
        { }

        protected abstract void Process(T1 component1, T2 component2);

        protected internal override void Process(EntityComponent[] components) => Process((T1)components[0], (T2)components[1]);
    }

    public abstract class EntitySystem<T1, T2, T3> : EntitySystem where T1 : class where T2 : class where T3 : class
    {
        protected EntitySystem(GameLoopType loopType = GameLoopType.Update) : base(new[] { typeof(T1), typeof(T2), typeof(T3) }, loopType)
        { }

        protected abstract void Process(T1 component1, T2 component2, T3 component3);

        protected internal override void Process(EntityComponent[] components) => Process((T1)components[0], (T2)components[1], (T3)components[2]);
    }
}
