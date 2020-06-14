using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Ptolemy
{
    class Physics
    {
        //Need to add a shortening length at some point to prevent singularities
        //We can perform coordinate scaling by modification of the gravitational constant
        private double gravitationalConstant;
        private double stepSize;
        private double shorteningLength;
        public Body[] Bodies { get; set; }

        public Physics(double stepSize, Body[] bodies, double gravitationalConstant, double shorteningLength)
        {
            this.stepSize = stepSize;
            this.Bodies = bodies;
            this.gravitationalConstant = gravitationalConstant;
            this.shorteningLength = shorteningLength;

            InitialiseRK4();
        }

        public void UpdateStep(int iters)
        {
            int bodyNum = Bodies.Length;

            double[] displacement = new double[3];
            double[] force = new double[3];
            double[] forcePrediction = new double[3];
            double[] velocityPrediction = new double[3];
            double[] newVelocity = new double[3];
            double norm;

            double[][] positions = new double[bodyNum][];
            double[][] velocities = new double[bodyNum][];
            double[] masses  = new double[bodyNum];

            double[][][] ForceEvals = new double[bodyNum][][];
            double[][][] VelEvals = new double[bodyNum][][];
            double[][] posPredictions = new double[bodyNum][];


            //Load the bodies and populate the arrays
            for (int i = 0; i < bodyNum; i++)
            {
                positions[i] = Bodies[i].Position;
                velocities[i] = Bodies[i].Velocity;
                masses[i] = Bodies[i].Mass;

                ForceEvals[i] = Bodies[i].ForceEvaluations;
                VelEvals[i] = Bodies[i].VelocityEvaluations;
            }


            for (int iter = 0; iter < iters; iter++)
            {
                for (int i = 0; i < bodyNum; i++)
                {
                    Array.Clear(force, 0, force.Length);

                    for (int j = 0; j < bodyNum; j++)
                    {
                        if (i == j) { continue; }
                        displacement = positions[i].Subtract(positions[j]);
                        norm = displacement.Norm(shorteningLength);
                        force = force.Subtract(displacement.ScalarProduct(stepSize * gravitationalConstant * masses[j]
                                            / (norm * norm * norm)));
                    }

                    

                    velocityPrediction = velocities[i].Add((force.ScalarProduct(55.0))
                                                  .Subtract(ForceEvals[i][0].ScalarProduct(59.0))
                                                  .Add(ForceEvals[i][1].ScalarProduct(37.0))
                                                  .Subtract(ForceEvals[i][2].ScalarProduct(9.0))
                                                  .ScalarProduct(stepSize / 24.0));

                    posPredictions[i] = positions[i].Add(velocityPrediction.ScalarProduct(251.0)
                                                   .Add(velocities[i].ScalarProduct(646.0))
                                                   .Subtract(VelEvals[i][0].ScalarProduct(264.0))
                                                   .Add(VelEvals[i][1].ScalarProduct(106.0))
                                                   .Subtract(VelEvals[i][2].ScalarProduct(19.0))
                                                   .ScalarProduct(stepSize / 720.0));
                    ForceEvals[i].Push(force);


                }

                for (int i = 0; i < bodyNum; i++)
                {
                    Array.Clear(forcePrediction, 0, forcePrediction.Length);

                    for (int j = 0; j < bodyNum; j++)
                    {
                        if (i == j) { continue; }
                        displacement = positions[i].Subtract(posPredictions[j]);
                        norm = displacement.Norm(shorteningLength);
                        forcePrediction = forcePrediction.Subtract(displacement.ScalarProduct(stepSize * gravitationalConstant * masses[j]
                                            / (norm * norm * norm)));
                    }

                    newVelocity = velocities[i].Add(forcePrediction.ScalarProduct(251.0)
                                                   .Add(ForceEvals[i][0].ScalarProduct(646.0))
                                                   .Subtract(ForceEvals[i][1].ScalarProduct(264.0))
                                                   .Add(ForceEvals[i][2].ScalarProduct(106.0))
                                                   .Subtract(ForceEvals[i][3].ScalarProduct(19.0))
                                                   .ScalarProduct(stepSize / 720.0));

                    positions[i] = positions[i].Add(newVelocity.ScalarProduct(251.0)
                                                   .Add(velocities[i].ScalarProduct(646.0))
                                                   .Subtract(VelEvals[i][0].ScalarProduct(264.0))
                                                   .Add(VelEvals[i][1].ScalarProduct(106.0))
                                                   .Subtract(VelEvals[i][2].ScalarProduct(19.0))
                                                   .ScalarProduct(stepSize / 720.0));
                    VelEvals[i].Push(velocities[i]);
                    velocities[i] = newVelocity;

                }
            }

            //Push data back into Body objects
            for (int i = 0; i < bodyNum; i++)
            {
                Bodies[i].Position = positions[i];
                Bodies[i].Velocity = velocities[i];

                Bodies[i].ForceEvaluations = ForceEvals[i];
                Bodies[i].VelocityEvaluations = VelEvals[i];
            }
        }
        void InitialiseRK4()
        /*
         Creates the initial points using RK4
         */
        {
            double[,][] k = new double[Bodies.Length, 4][];
            double[,][] l = new double[Bodies.Length, 4][];

            double[] displacement = new double[3];
            double norm = 0.0;
            //Initialise the k and l arrays
            for (int i = 0; i < k.GetLength(0); i++)
            {
                for (int j = 0; j < k.GetLength(1); j++)
                {
                    k[i,j] = new double[3];
                    l[i, j] = new double[3];
                }
            }

            for (int i = 0; i < 3; i++)
            {
                //Find k1/l1
                for (int j = 0; j < Bodies.Length; j++)
                {
                    k[j, 1] = Bodies[j].Velocity.ScalarProduct(stepSize);

                    for (int j2 = 0; j2 < Bodies.Length; j2++)
                    {
                        if(j == j2) { continue; }
                        displacement = Bodies[j].Position.Subtract(Bodies[j2].Position);
                        norm = displacement.Norm(shorteningLength);

                        l[j, 1] = l[j, 1].Subtract(displacement
                                                    .ScalarProduct(stepSize * gravitationalConstant * Bodies[j2].Mass 
                                                    / (norm * norm * norm)));
                    }
                    //Store g_n
                    Bodies[j].pushForce(l[j, 1]);
                    Bodies[j].pushVel();
                }

                //Find k2/l2 and k3/l3
                for (int i2 = 1; i2 <= 2; i2++)
                {
                    for (int j = 0; j < Bodies.Length; j++)
                    {
                        k[j, i2] = Bodies[j].Velocity.Add(l[j, i2-1].ScalarProduct(0.5)).ScalarProduct(stepSize);

                        for (int j2 = 0; j2 < Bodies.Length; j2++)
                        {
                            if (j == j2) { continue; }
                            displacement = Bodies[j].Position.Add(k[j, i2-1].ScalarProduct(0.5))
                                .Subtract(Bodies[j2].Position.Add(k[j2, i2-1].ScalarProduct(0.5)));
                            norm = displacement.Norm(shorteningLength);

                            l[j, i2] = l[j, i2].Subtract(displacement
                                                        .ScalarProduct(stepSize * gravitationalConstant * Bodies[j2].Mass
                                                        / (norm * norm * norm)));
                        }
                    }
                }
                //Find k4/l4
                for (int j = 0; j < Bodies.Length; j++)
                {
                    k[j, 3] = Bodies[j].Velocity.Add(l[j, 2]).ScalarProduct(stepSize);

                    for (int j2 = 0; j2 < Bodies.Length; j2++)
                    {
                        if (j == j2) { continue; }
                        displacement = Bodies[j].Position.Add(k[j, 2])
                            .Subtract(Bodies[j2].Position.Add(k[j2, 2]));
                        norm = displacement.Norm(shorteningLength);

                        l[j, 3] = l[j, 3].Subtract(displacement
                                                    .ScalarProduct(stepSize * gravitationalConstant * Bodies[j2].Mass
                                                    / (norm * norm * norm)));
                    }
                }

                //Update Position and Velocity vectors
                for (int j = 0; j < Bodies.Length; j++)
                {
                    Bodies[j].Position = Bodies[j].Position.Add(k[j, 0]
                                                           .Add(k[j, 1].ScalarProduct(2.0))
                                                           .Add(k[j, 2].ScalarProduct(2.0))
                                                           .Add(k[j, 3]).ScalarProduct(1.0 / 6.0));

                    Bodies[j].Velocity = Bodies[j].Velocity.Add(l[j, 0]
                                                           .Add(l[j, 1].ScalarProduct(2.0))
                                                           .Add(l[j, 2].ScalarProduct(2.0))
                                                           .Add(l[j, 3]).ScalarProduct(1.0 / 6.0));
                }
            }
        }
    }
}
