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
        public Body[] Bodies { get; set; }

        public Physics(double stepSize, Body[] bodies, double gravitationalConstant)
        {
            this.stepSize = stepSize;
            this.Bodies = bodies;
            this.gravitationalConstant = gravitationalConstant;

            InitialiseRK4();
        }

        public void UpdateStep()
        {
            double[] displacement = new double[3];
            double[] force = new double[3];
            double[] forcePrediction = new double[3];
            double[] velocityPrediction = new double[3];
            double[] positionPrediction = new double[3];
            double[] newVelocity = new double[3];

            double norm;

            for(int i = 0; i < Bodies.Length; i++)
            {
                for (int j = 0; j < Bodies.Length; j++)
                {
                    if( i == j){ continue; }
                    displacement = Bodies[i].Position.Subtract(Bodies[j].Position);
                    norm = displacement.Norm();
                    force = force.Subtract(displacement.ScalarProduct(stepSize * gravitationalConstant * Bodies[j].Mass
                                        / (norm * norm * norm)));
                }
                
                velocityPrediction = Bodies[i].Velocity.Add((force.ScalarProduct(55.0))
                                              .Subtract(Bodies[i].ForceEvaluations[0].ScalarProduct(59.0))
                                              .Add(Bodies[i].ForceEvaluations[1].ScalarProduct(37.0))
                                              .Subtract(Bodies[i].ForceEvaluations[2].ScalarProduct(9.0))
                                              .ScalarProduct(stepSize / 24.0));
                //Console.WriteLine(force[0].ToString());

                positionPrediction = Bodies[i].Position.Add(velocityPrediction.ScalarProduct(251.0)
                                               .Add(Bodies[i].Velocity.ScalarProduct(646.0))
                                               .Subtract(Bodies[i].VelocityEvaluations[0].ScalarProduct(264.0))
                                               .Add(Bodies[i].VelocityEvaluations[1].ScalarProduct(106.0))
                                               .Subtract(Bodies[i].VelocityEvaluations[2].ScalarProduct(19.0))
                                               .ScalarProduct(stepSize / 720.0));
                //Need to check the eval arrays most likely
                for (int j = 0; j < Bodies.Length; j++)
                {
                    if (i == j) { continue; }
                    displacement = positionPrediction.Subtract(Bodies[j].Position);
                    norm = displacement.Norm();
                    forcePrediction = forcePrediction.Subtract(displacement.ScalarProduct(stepSize * gravitationalConstant * Bodies[j].Mass
                                        / (norm * norm * norm)));
                }

                newVelocity = Bodies[i].Velocity.Add(forcePrediction.ScalarProduct(251.0)
                                               .Add(force.ScalarProduct(646.0))
                                               .Subtract(Bodies[i].ForceEvaluations[0].ScalarProduct(264.0))
                                               .Add(Bodies[i].ForceEvaluations[1].ScalarProduct(106.0))
                                               .Subtract(Bodies[i].ForceEvaluations[2].ScalarProduct(19.0))
                                               .ScalarProduct(stepSize / 720.0));

                Bodies[i].Position = Bodies[i].Position.Add(newVelocity.ScalarProduct(251.0)
                                               .Add(Bodies[i].Velocity.ScalarProduct(646.0))
                                               .Subtract(Bodies[i].VelocityEvaluations[0].ScalarProduct(264.0))
                                               .Add(Bodies[i].VelocityEvaluations[1].ScalarProduct(106.0))
                                               .Subtract(Bodies[i].VelocityEvaluations[2].ScalarProduct(19.0))
                                               .ScalarProduct(stepSize / 720.0));
                Bodies[i].pushVel();
                Bodies[i].Velocity = newVelocity;
                Bodies[i].pushForce(force);
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
                        norm = displacement.Norm();

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
                            norm = displacement.Norm();

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
                        norm = displacement.Norm();

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
