using System;
using System.Collections.Generic;
using System.Threading;
using ECS.Managers;
using Bitfield = System.UInt64;
using EntityComponent = System.Object;

namespace ECS
{
    public sealed class EntityWorld : IEquatable<EntityWorld>
    {
        static int _idSequence;
        internal readonly int Id;

        readonly Dictionary<int, List<EntityComponent[]>> _systemComponents = new Dictionary<int, List<EntityComponent[]>>();

        public EntityWorld(EntityWorldConfiguration configuration = null)
        {
            Id = Interlocked.Increment(ref _idSequence);

            Configuration = configuration ?? new EntityWorldConfiguration();
            EntityComponentManager = new EntityComponentManager(this);
            EntityManager = new EntityManager(this);
            EntitySystemManager = new EntitySystemManager(this);

            UpdateContext = new ProcessingContext { Dirty = true, Systems = EntitySystemManager.SystemsForUpdate };
            DrawContext = new ProcessingContext { Dirty = true, Systems = EntitySystemManager.SystemsForDraw };
        }        

        internal EntityWorldConfiguration Configuration { get; }
        internal EntityComponentManager EntityComponentManager { get; }
        internal EntityManager EntityManager { get; }
        internal EntitySystemManager EntitySystemManager { get; }

        internal ProcessingContext UpdateContext { get; }
        internal ProcessingContext DrawContext { get; }

        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan TotalTime { get; private set; }

        public Entity CreateEntity() => EntityManager.FetchEntityFromPool();
        public T CreateComponent<T>() where T : class => EntityComponentManager.CreateComponentFromPool<T>();

        public Dictionary<int, Entity>.ValueCollection GetEntities() => EntityManager.Entities.Values;
        public Dictionary<Type, EntitySystem>.ValueCollection GetSystems() => EntitySystemManager.Systems.Values;
        public EntityWorld AddSystem<T>(T system) where T : EntitySystem => EntitySystemManager.AddSystem(system);
        public EntitySystem RemoveSystem<T>() where T : EntitySystem => EntitySystemManager.RemoveSystem<T>();

        public void Update(TimeSpan elapsedTime)
        {
            ElapsedTime = elapsedTime;
            TotalTime = TotalTime.Add(elapsedTime);
            ProcessSystems(UpdateContext);
        }

        public void Draw()
        {
            ProcessSystems(DrawContext);
        }

        public EntityWorldState GetState()
        {
            var state = new EntityWorldState
            {
                TotalTime = TotalTime
            };
            EntityManager.GetState(state);
            return state;
        }

        public void RestoreState(EntityWorldState state)
        {
            EntityManager.RestoreState(state);
            TotalTime = state.TotalTime;
        }

        internal void SetAllContextsDirty()
        {
            UpdateContext.Dirty = true;
            DrawContext.Dirty = true;
        }

        void ProcessSystems(ProcessingContext context)
        {
            PrepareComponentsForSystems(context);
            for (int i = 0; i < context.Systems.Count; i++)
            {
                EntitySystem system = context.Systems[i];
                if (!system.Enabled) continue;

                foreach (EntityComponent[] componentSet in _systemComponents[system.Id])
                    system.Process(componentSet);
            }
        }

        void PrepareComponentsForSystems(ProcessingContext context)
        {
            if (context.Dirty)
            {
                for (int i = 0; i < context.Systems.Count; i++)
                    PrepareComponentsForSystem(context.Systems[i]);
                context.Dirty = false;
            }
        }

        void PrepareComponentsForSystem(EntitySystem system)
        {
            List<EntityComponent[]> allComponentsForSystem;
            if (!_systemComponents.TryGetValue(system.Id, out allComponentsForSystem))
            {
                allComponentsForSystem = new List<EntityComponent[]>();
                _systemComponents.Add(system.Id, allComponentsForSystem);
            }

            int componentSetCounter = 0;
            foreach (Entity entity in EntityManager.Entities.Values)
            {
                if (!entity.Enabled) continue;
                if ((entity.ComponentBits & system.ComponentBits) == system.ComponentBits)
                {
                    EntityComponent[] components;
                    if (allComponentsForSystem.Count <= componentSetCounter)
                    {
                        components = new EntityComponent[system.NumberOfBits];
                        allComponentsForSystem.Add(components);
                    }
                    else
                    {
                        components = allComponentsForSystem[componentSetCounter++];
                    }

                    entity.GetComponentsForComponentBits(system.ComponentBits, system.NumberOfBits, components);
                }
            }
        }

        public bool Equals(EntityWorld other) => Id == other?.Id;
        public override bool Equals(object obj) => Equals(obj as EntityWorld);
        public override int GetHashCode() => Id;

        internal class ProcessingContext
        {
            internal bool Dirty;
            internal List<EntitySystem> Systems;
        }
    }

    public class EntityWorldState
    {
        public TimeSpan TotalTime { get; set; }
        public EntityState[] Entities { get; set; }
    }

    public class EntityWorldConfiguration
    {
        int _initialEntityPoolCapacity = 4;
        public int InitialEntityPoolCapacity
        {
            get { return _initialEntityPoolCapacity; }
            set { _initialEntityPoolCapacity = Math.Max(value, 1); }
        }

        int _initialComponentPoolCapacity = 4;
        public int InitialComponentPoolCapacity
        {
            get { return _initialComponentPoolCapacity; }
            set { _initialComponentPoolCapacity = Math.Max(value, 1); }
        }
    }
}
