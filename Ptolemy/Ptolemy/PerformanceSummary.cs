using System;
using System.Collections.Generic;
using System.Text;

namespace Ptolemy
{
    class PerformanceSummary
    {
        public int IterationsTot { get; }
        public int IterationsRun { get; }
        public double EnergyBefore { get; }
        public double EnergyAfter { get; }
        public double EnergyDiff { get; }
        public double Time { get; }
        public double ItersPerSecond { get; }

        public PerformanceSummary(int IterationsTot, int IterationsRun, double EnergyBefore, double EnergyAfter,
            double EnergyDiff, double Time, double ItersPerSecond)
        {
            this.IterationsTot = IterationsTot;
            this.IterationsRun = IterationsRun;
            this.EnergyBefore = EnergyBefore;
            this.EnergyAfter = EnergyAfter;
            this.EnergyDiff = EnergyDiff;
            this.Time = Time;
            this.ItersPerSecond = ItersPerSecond;
        }

        public void Print()
        {
            Console.WriteLine("================================Performance Summary====================================");
            Console.WriteLine("Iterations This Run: {0}", IterationsRun);
            Console.WriteLine("Iterations Total: {0}", IterationsTot);
            Console.WriteLine();

            Console.WriteLine("Time elapsed: {0}", Time);
            Console.WriteLine("Iterations per second: {0}", ItersPerSecond);
            Console.WriteLine();

            Console.WriteLine("Initial Energy: {0}", EnergyBefore);
            Console.WriteLine("Final Energy: {0}", EnergyAfter);
            Console.WriteLine("Energy Delta: {0}", EnergyDiff);
            Console.WriteLine("=======================================================================================");
        }
    }
}
