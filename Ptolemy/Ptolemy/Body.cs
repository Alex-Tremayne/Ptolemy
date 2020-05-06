using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Ptolemy
{
    class Body
    {
        public double Mass { get; }

        public double[] Position { get; set; }
        public double[] Velocity { get; set; }


        public double[][] ForceEvaluations { get; set; }//Array of net force at previous time steps
        public double[][] VelocityEvaluations { get; set; }//Array of velocities at previous time steps

        public void pushVel()
        {
            //Move all elements in the array to the right and insert the new value
            Array.Copy(VelocityEvaluations, 0, VelocityEvaluations, 1, 2);
            VelocityEvaluations[0] = Velocity;
        }
        public void pushForce(double[] force)
        {
            //Move all elements in the array to the right and insert the new value
            Array.Copy(ForceEvaluations, 0, ForceEvaluations, 1, 2);
            ForceEvaluations[0] = force;
        }

        public Body(double mass, double[] position, double[] velocity)
        {
            this.Mass = mass;

            this.Position = position;
            this.Velocity = velocity;

            ForceEvaluations = new double[3][];
            VelocityEvaluations = new double[3][];

            for (int i = 0; i < VelocityEvaluations.Length; i++)
            {
                VelocityEvaluations[i] = new double[3];
            }

            for (int i = 0; i < ForceEvaluations.Length; i++)
            {
                ForceEvaluations[i] = new double[3];
            }


        }
    }
}
