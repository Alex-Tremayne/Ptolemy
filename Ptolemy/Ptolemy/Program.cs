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
            double[] pos = { 0, 0, 0 };
            double[] vel = new double[3];

            for(int i = 0; i < bodyNum; i++)
            {
                pos[0] = lims * random.NextDouble();
                pos[1] = lims * random.NextDouble();
                pos[2] = lims * random.NextDouble();
                bodies[i] = new Body(1.0, pos, vel);
            }
            
            Physics simulation = new Physics(stepSize, bodies, gravConst);

            //double energy = 0.0;
            //double[] displacement = new double[3];
            //double norm = 0.0;
            //for(int i = 0; i < bodies.Length; i++)
            //{
            //    energy += simulation.Bodies[i].Mass * simulation.Bodies[i].Velocity.Norm()
            //        * simulation.Bodies[i].Velocity.Norm() * 0.5;
            //    for (int j = 0; j < bodies.Length; j++)
            //    {
            //        if ( i == j) { continue; }
            //        displacement = simulation.Bodies[i].Position.Subtract(simulation.Bodies[j].Position);
            //        norm = displacement.Norm();
            //        energy += (stepSize * gravConst * simulation.Bodies[i].Mass * simulation.Bodies[j].Mass
            //                            / (norm));
            //    }
            //}

            //double energyInit = energy;
            //Console.Write("Initial Energy: ");
            //Console.WriteLine(energy.ToString());
            Console.WriteLine("Simulating");

            Stopwatch stopwatch = Stopwatch.StartNew();
            simulation.UpdateStep(200);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine(200.0 / (double)stopwatch.ElapsedMilliseconds * 1000.0);


            //energy = 0.0;
            //for (int i = 0; i < bodies.Length; i++)
            //{
            //    energy += simulation.Bodies[i].Mass * simulation.Bodies[i].Velocity.Norm()
            //        * simulation.Bodies[i].Velocity.Norm() * 0.5;
            //    for (int j = 0; j < bodies.Length; j++)
            //    {
            //        if (i == j) { continue; }
            //        displacement = simulation.Bodies[i].Position.Subtract(simulation.Bodies[j].Position);
            //        norm = displacement.Norm();
            //        energy += (stepSize * gravConst * simulation.Bodies[i].Mass * simulation.Bodies[j].Mass
            //                            / (norm));
            //    }
            //}
            //Console.Write("Final Energy: ");
            //Console.WriteLine(energy.ToString());
            //Console.WriteLine();
            //Console.Write("Error %: ");
            //Console.WriteLine((100.0*(energy-energyInit)/energy).ToString());
        }
    }
}
