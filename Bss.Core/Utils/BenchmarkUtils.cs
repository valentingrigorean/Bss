using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bss.Core.Utils
{
    public static class BenchmarkUtils
    {
        public static void Benchmark(Action action, [CallerMemberName] string messageConsole = "")
        {
            var timer = new Stopwatch();
            Debug.WriteLine(messageConsole);
            timer.Start();
            action();
            timer.Stop();
            Debug.WriteLine("Time elapsed: {0} {1}", timer.Elapsed.ToString("G"),messageConsole);
        }
    }
}
