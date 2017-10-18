using ECS.UnitTests.Fakes;
using System;
using System.Linq;
using Xunit;

namespace ECS.UnitTests
{
    public class EntityWorldFacts
    {
        public class TheAddSystemMethod : EntityWorldFacts
        {
            [Fact]
            public void ThrowsArgumentNullForNullSystem()
            {
                EntityWorld target = EmptyWorld();

                Assert.Throws<ArgumentNullException>(() => target.AddSystem<EntitySystem>(null));
            }

            [Fact]
            public void AddsAndReturnsItselfForSystem()
            {
                EntityWorld target = EmptyWorld();

                Assert.Equal(target, target.AddSystem(new FakeComponent1System()));
                Assert.Single(target.GetSystems());
            }

            [Fact]
            public void ThrowsInvalidOperationForTwoSystemsOfSameType()
            {
                EntityWorld target = EmptyWorld();
                target.AddSystem(new FakeComponent1System());

                Assert.Throws<InvalidOperationException>(() => target.AddSystem(new FakeComponent1System()));
            }

            [Fact]
            public void ThrowsInvalidOperationForSystemInAnotherWorld()
            {
                EntityWorld target1 = EmptyWorld();
                EntityWorld target2 = EmptyWorld();
                var system = new FakeComponent1System();
                target1.AddSystem(system);

                Assert.Throws<InvalidOperationException>(() => target2.AddSystem(system));
            }
        }

        public class TheRemoveSystemMethod : EntityWorldFacts
        {
            [Fact]
            public void ReturnsNullForNonExistentType()
            {
                EntityWorld target = EmptyWorld();

                Assert.Null(target.RemoveSystem<FakeComponent1System>());
            }

            [Fact]
            public void RemovesAndReturnsSystemForExistentType()
            {
                EntityWorld target = EmptyWorld();
                var system = new FakeComponent1System();
                target.AddSystem(system);

                Assert.Equal(system, target.RemoveSystem<FakeComponent1System>());
                Assert.Empty(target.GetSystems());
            }
        }

        public class TheEqualsMethod : EntityWorldFacts
        {
            [Fact]
            public void ReturnsTrueForSameWorlds()
            {
                EntityWorld target = EmptyWorld();

                Assert.True(target.Equals(target));
            }

            [Fact]
            public void ReturnsFalseForDifferentWorlds()
            {
                EntityWorld target1 = EmptyWorld();
                EntityWorld target2 = EmptyWorld();

                Assert.False(target1.Equals(target2));
            }
        }

        public class TheGetStateAndRestoreStateMethods : EntityWorldFacts
        {
            [Fact]
            public void TransferComponentFromOneWorldToAnother()
            {
                EntityWorld target1 = EmptyWorld();
                Entity entity = target1.CreateEntity();
                entity.AddComponent(new FakeComponent1 {  Processed = true });
                EntityWorld target2 = EmptyWorld();

                target2.RestoreState(target1.GetState());

                Assert.True(target2.GetEntities().First().GetComponent<FakeComponent1>().Processed);
            }

            [Fact]
            public void TransferEntityStateFromOneWorldToAnother()
            {
                EntityWorld target1 = EmptyWorld();
                Entity entity = target1.CreateEntity();
                entity.Enabled = false;
                EntityWorld target2 = EmptyWorld();

                target2.RestoreState(target1.GetState());

                Assert.False(target2.GetEntities().First().Enabled);
            }

            [Fact]
            public void TransferWorldStateFromOneWorldToAnother()
            {
                EntityWorld target1 = EmptyWorld();
                target1.Update(TimeSpan.FromSeconds(5));
                EntityWorld target2 = EmptyWorld();

                target2.RestoreState(target1.GetState());

                Assert.Equal(target1.TotalTime, target2.TotalTime);
            }

            [Fact]
            public void TransferDeepCopyOfComponentFromOneWorldToAnother()
            {
                EntityWorld target1 = EmptyWorld();
                Entity entity = target1.CreateEntity();
                var component = new FakeComponent1();
                entity.AddComponent(component);
                EntityWorld target2 = EmptyWorld();

                target2.RestoreState(target1.GetState());
                component.Processed = true;

                Assert.False(target2.GetEntities().First().GetComponent<FakeComponent1>().Processed);
            }

            [Fact]
            public void RestoreStateToAPreviousStateWithinSameWorld()
            {
                EntityWorld target = EmptyWorld();
                Entity entity = target.CreateEntity();
                var component = new FakeComponent1();
                entity.AddComponent(component);

                var state = target.GetState();
                component.Processed = true;
                target.RestoreState(state);

                Assert.False(target.GetEntities().First().GetComponent<FakeComponent1>().Processed);
            }
        }

        public class TheCreateEntityMethod
        {
            [Fact]
            public void ReusesPooledEntities()
            {
                var target = new EntityWorld(new EntityWorldConfiguration { InitialEntityPoolCapacity = 1 });
                Entity entity1 = target.CreateEntity();
                entity1.Dispose();

                Entity entity2 = target.CreateEntity();

                Assert.Equal(entity1, entity2);
            }
        }

        public class TheCreateComponentMethod
        {
            [Fact]
            public void ReusesPooledComponents()
            {
                var target = new EntityWorld(new EntityWorldConfiguration { InitialComponentPoolCapacity = 1 });
                Entity entity = target.CreateEntity();
                FakeComponent1 component1 = target.CreateComponent<FakeComponent1>();
                entity.AddComponent(component1);
                entity.Dispose();

                FakeComponent1 component2 = target.CreateComponent<FakeComponent1>();

                Assert.Equal(component1, component2);
            }
        }

        protected EntityWorld EmptyWorld()
        {
            return new EntityWorld();
        }
    }
}
