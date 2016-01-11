using ECS.UnitTests.Fakes;
using System;
using Xunit;

namespace ECS.UnitTests
{
    public class EntityFacts
    {
        public class TheAddComponentMethod : EntityFacts
        {
            [Fact]
            public void ThrowsArgumentNullForNullComponent()
            {
                Entity target = EmptyEntity();

                Assert.Throws<ArgumentNullException>(() => target.AddComponent(null));
            }

            [Fact]
            public void AddsComponentAndReturnsItselfForComponent()
            {
                Entity target = EmptyEntity();

                Assert.Equal(target, target.AddComponent(new FakeComponent1()));
                Assert.Equal(1, target.GetComponents().Count);
            }

            [Fact]
            public void ThrowsInvalidOperationForTwoComponentsOfSameType()
            {
                Entity target = EmptyEntity();
                target.AddComponent(new FakeComponent1());

                Assert.Throws<InvalidOperationException>(() => target.AddComponent(new FakeComponent1()));
            }
        }

        public class TheGetComponentMethod : EntityFacts
        {
            [Fact]
            public void ReturnsNullForNonExistentType()
            {
                Entity target = EmptyEntity();

                Assert.Null(target.GetComponent<FakeComponent1>());
            }            

            [Fact]
            public void ReturnsComponentForExistentType()
            {
                Entity target = EmptyEntity();
                var component = new FakeComponent1();
                target.AddComponent(component);

                Assert.Equal(component, target.GetComponent<FakeComponent1>());
            }
        }

        public class TheRemoveComponentMethod : EntityFacts
        {
            [Fact]
            public void ReturnsNullForNonExistentType()
            {
                Entity target = EmptyEntity();

                Assert.Null(target.RemoveComponent<FakeComponent1>());
            }

            [Fact]
            public void RemovesAndReturnsComponentForExistentType()
            {
                Entity target = EmptyEntity();
                var component = new FakeComponent1();
                target.AddComponent(component);

                Assert.Equal(component, target.RemoveComponent<FakeComponent1>());
                Assert.Equal(0, target.GetComponents().Count);
            }
        }

        public class TheEqualsMethod : EntityFacts
        {
            [Fact]
            public void ReturnsTrueForSameEntities()
            {
                Entity target = EmptyEntity();

                Assert.True(target.Equals(target));
            }

            [Fact]
            public void ReturnsFalseForDifferentEntities()
            {
                Entity target1 = EmptyEntity();
                Entity target2 = EmptyEntity();

                Assert.False(target1.Equals(target2));
            }

            [Fact]
            public void ReturnsFalseForEntitiesFromDifferentWorlds()
            {
                var world1 = new EntityWorld();
                var world2 = new EntityWorld();
                Entity target1 = world1.CreateEntity();
                Entity target2 = world2.CreateEntity();

                Assert.False(target1.Equals(target2));
            }
        }
        
        readonly EntityWorld _world = new EntityWorld();
        protected Entity EmptyEntity()
        {            
            return _world.CreateEntity();
        }
    }
}
