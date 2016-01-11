using System;
using ECS.PerformanceTests.Components;

namespace ECS.PerformanceTests.Systems
{
    class SquareRootSystem : EntitySystem<IntegerComponent>
    {
        protected override void Process(IntegerComponent component)
        {
            component.Value = (int)Math.Sqrt(component.Value);
        }
    }
}
