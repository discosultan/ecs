using System;
using ECS.PerformanceTests.Components;
using ECS.PerformanceTests.Systems;

namespace ECS.PerformanceTests.Measurements
{
    class ProcessSystems : Measurement
    {
        const int NumUpdates = 10000;
        const int NumEntities = 10000;

        readonly Random _rnd = new Random();
        readonly EntityWorld _world = new EntityWorld();

        public ProcessSystems()
        {
            for (int i = 0; i < NumEntities; i++)
                _world.CreateEntity().AddComponent(new IntegerComponent { Value = _rnd.Next() });
            _world.AddSystem(new SquareRootSystem());
        }

        public override string Name { get; } = $"1 system, {NumEntities} entities, {NumUpdates} updates";
        protected override void PerformAction()
        {
            for (int i = 0; i < NumUpdates; i++)
                _world.Update(TimeSpan.Zero);
        }
    }
}
