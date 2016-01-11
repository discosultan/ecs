namespace ECS.UnitTests.Fakes
{
    class FakeComponent1System : EntitySystem<FakeComponent1>
    {
        protected override void Process(FakeComponent1 component)
        {
            component.Processed = true;
        }
    }

    class FakeComponent1And2System : EntitySystem<FakeComponent1, FakeComponent2>
    {
        protected override void Process(FakeComponent1 component1, FakeComponent2 component2)
        {
            component1.Processed = true;
            component2.Processed = true;
        }
    }

    class FakeComponent1And2And3System : EntitySystem<FakeComponent1, FakeComponent2, FakeComponent3>
    {
        protected override void Process(FakeComponent1 component1, FakeComponent2 component2, FakeComponent3 component3)
        {
            component1.Processed = true;
            component2.Processed = true;
            component3.Processed = true;
        }
    }

    class FakeComponentDrawSystem : EntitySystem<FakeComponent1>
    {
        public FakeComponentDrawSystem() : base(GameLoopType.Draw)
        {            
        }

        protected override void Process(FakeComponent1 component)
        {
            component.Processed = true;
        }
    }
}
