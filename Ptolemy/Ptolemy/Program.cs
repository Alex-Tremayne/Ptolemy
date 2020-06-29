using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ptolemy
{
    class Program
    {
        static void Main(string[] args)
        {
            
            double stepSize = 0.001;
            double gravConst = 1.0;

            int bodyNum = 100;
            double lims = 1000;

            Body[] bodies = new Body[bodyNum];

            Random random = new Random();
            //double[] pos = { 0, 0, 0 };
            double[] vel = new double[3];

            for(int i = 0; i < bodyNum; i++)
            {
                //Apparently we need to do this, otherwise all the bodies will have the last position calculated
                //I have no idea why :/
                double[] pos = { 0, 0, 0 };

                pos[0] = lims * random.NextDouble();
                pos[1] = lims * random.NextDouble();
                pos[2] = lims * random.NextDouble();

                bodies[i] = new Body(1.0, pos, vel);
            }




            Physics simulation = new Physics(stepSize, bodies, gravConst, 1.0, new double[] { lims, lims, lims }, SummarisePerformance: true);

            int iterations = 2000;
            
            Console.WriteLine("Simulating");
            

            simulation.RunDirect(iterations);

        }
    }
}
