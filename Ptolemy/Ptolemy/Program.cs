using System;
namespace Ptolemy
{
    class Program
    {
        static void Main(string[] args)
        {
            Body[] bodies = new Body[2];
            double stepSize = 0.1; //Will add code to check the total energy as a measure of accuracy

            double[] temp = { 0, 0, 0 };
            bodies[0] = new Body(1, temp, temp);
            double[] temp2 = { 10, 0, 0 };
            bodies[1] = new Body(1, temp2, temp);

            Physics simulation = new Physics(stepSize, bodies,1);

            for(int i = 0; i < 100; i++)
            {
                simulation.UpdateStep();
                Console.WriteLine(simulation.Bodies[1].Position[0].ToString());
            }

        }
    }
}
