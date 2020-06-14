using System;
using System.Diagnostics;

namespace Ptolemy
{
    class Program
    {
        static void Main(string[] args)
        {
            
            double stepSize = 0.001; //Will add code to check the total energy as a measure of accuracy
            double gravConst = 1.0;

            int bodyNum = 10;
            double lims = 1000;

            Body[] bodies = new Body[bodyNum];

            Random random = new Random();
            //double[] pos = { 0, 0, 0 };
            double[] vel = new double[3];

            for(int i = 0; i < bodyNum; i++)
            {
                //Apparently we need to do this, otherwise bodies will have the last position calculated
                //I have no idea why :/
                double[] pos = { 0, 0, 0 };

                pos[0] = lims * random.NextDouble();
                pos[1] = lims * random.NextDouble();
                pos[2] = lims * random.NextDouble();

                bodies[i] = new Body(1.0, pos, vel);
            }

            Console.WriteLine(bodies[0].Position[0].ToString());


            Physics simulation = new Physics(stepSize, bodies, gravConst, 1.0);

            double energyInit = simulation.GetEnergy();

            int iterations = 2000;
            
            Console.WriteLine("Simulating");
            Console.WriteLine();

            Stopwatch stopwatch = Stopwatch.StartNew();
            simulation.UpdateStep(iterations);
            stopwatch.Stop();

            Console.WriteLine("Time elapsed: {0}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Iterations per second: {0}", iterations / (double)stopwatch.ElapsedMilliseconds * 1000.0);
            Console.WriteLine();

            double energy = simulation.GetEnergy();
            Console.WriteLine("Initial Energy: {0}", energyInit);
            Console.WriteLine("Final Energy: {0}", energy);
            Console.WriteLine("Energy Delta: {0}", energy - energyInit);

            bodies = simulation.getBodies();
            Console.WriteLine(bodies[0].Position[0].ToString());

        }
    }
}
