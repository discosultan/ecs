using System;
using System.Diagnostics;

namespace ECS.PerformanceTests
{
    abstract class Measurement
    {
        public TimeSpan Go()
        {
            GC.WaitForPendingFinalizers();
            var sw = Stopwatch.StartNew();
            PerformAction();
            sw.Stop();
            return sw.Elapsed;
        }

        public abstract string Name { get; }

        protected abstract void PerformAction();
    }
}
