using System;
using ECS.UnitTests.Fakes;
using Xunit;

namespace ECS.UnitTests
{
    public class EntitySystemFacts
    {
        public class TheProcessMethod : EntitySystemFacts
        {
            [Fact]
            public void IsExecutedForComponentOfInterest()
            {
                var target = new FakeComponent1System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component);

                world.Update(TimeSpan.Zero);

                Assert.True(component.Processed);
            }

            [Fact]
            public void IsNotExecutedForComponentOfNoInterest()
            {
                var target = new FakeComponent1System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent2();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component);

                world.Update(TimeSpan.Zero);

                Assert.False(component.Processed);
            }

            [Fact]
            public void IsExecutedForTwoComponentsOfInterest()
            {
                var target = new FakeComponent1And2System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component1 = new FakeComponent1();
                var component2 = new FakeComponent2();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component1);
                entity.AddComponent(component2);

                world.Update(TimeSpan.Zero);

                Assert.True(component1.Processed);
                Assert.True(component2.Processed);
            }

            [Fact]
            public void IsNotExecutedForOnlyOneOfTwoComponentsOfInterest()
            {
                var target = new FakeComponent1And2System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component1 = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component1);

                world.Update(TimeSpan.Zero);

                Assert.False(component1.Processed);
            }

            [Fact]
            public void IsExecutedForThreeComponentsOfInterest()
            {
                var target = new FakeComponent1And2And3System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component1 = new FakeComponent1();
                var component2 = new FakeComponent2();
                var component3 = new FakeComponent3();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component1);
                entity.AddComponent(component2);
                entity.AddComponent(component3);

                world.Update(TimeSpan.Zero);

                Assert.True(component1.Processed);
                Assert.True(component2.Processed);
                Assert.True(component3.Processed);
            }

            [Fact]
            public void IsExecutedForDrawGameLoopSystem()
            {
                var target = new FakeComponentDrawSystem();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component);

                world.Draw();

                Assert.True(component.Processed);
            }

            [Fact]
            public void IsNotExecutedForUpdateSystemInDrawLoop()
            {
                var target = new FakeComponentDrawSystem();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component);

                world.Update(TimeSpan.Zero);

                Assert.False(component.Processed);
            }

            [Fact]
            public void IsNotExecutedForDrawSystemInUpdateLoop()
            {
                var target = new FakeComponent1System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component);

                world.Draw();

                Assert.False(component.Processed);
            }

            [Fact]
            public void IsNotExecutedForDisabledSystem()
            {
                var target = new FakeComponent1System { Enabled = false };
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.AddComponent(component);

                world.Update(TimeSpan.Zero);

                Assert.False(component.Processed);
            }

            [Fact]
            public void IsNotExecutedForDisabledEntity()
            {
                var target = new FakeComponent1System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();
                entity.Enabled = false;
                entity.AddComponent(component);

                world.Update(TimeSpan.Zero);

                Assert.False(component.Processed);
            }

            [Fact]
            public void IsNotExecutedForDisposedEntity()
            {
                var target = new FakeComponent1System();
                var world = new EntityWorld();
                world.AddSystem(target);
                var component = new FakeComponent1();
                Entity entity = world.CreateEntity();                
                entity.AddComponent(component);
                entity.Dispose();

                world.Update(TimeSpan.Zero);

                Assert.False(component.Processed);
            }
        }
    }
}
