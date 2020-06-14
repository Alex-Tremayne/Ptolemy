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

            int bodyNum = 100;
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



            Console.WriteLine(ReferenceEquals(bodies[0], bodies[1]));

            Physics simulation = new Physics(stepSize, bodies, gravConst, 1.0);

            

            double energyInit = simulation.GetEnergy();
            
            Console.WriteLine("Simulating");

            Stopwatch stopwatch = Stopwatch.StartNew();
            simulation.UpdateStep(200);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine(200.0 / (double)stopwatch.ElapsedMilliseconds * 1000.0);

            double energy = simulation.GetEnergy();
            Console.WriteLine("Initial Energy: {0}", energyInit);
            Console.WriteLine("Final Energy: {0}", energy);
            Console.WriteLine("Energy Delta: {0}", energy - energyInit);

        }
    }
}
