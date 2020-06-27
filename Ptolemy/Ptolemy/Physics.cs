using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ptolemy
{
    class Physics
    {
        //We can perform coordinate scaling by modification of the gravitational constant
        private double gravitationalConstant;
        private double stepSize;
        private double shorteningLength;
        
        private Body[] Bodies{ get; set; }

        int bodyNum;

        double[][] positions;
        double[][] velocities;
        double[] masses;

        double[][][] ForceEvals;
        double[][][] VelEvals;
        double[][] posPredictions;


        //FMM variables

        int MaxBodiesPerBox;

            

        public Physics(double stepSize, Body[] bodies, double gravitationalConstant, double shorteningLength, int MaxBodiesPerBox = 10)
        {
            this.stepSize = stepSize;
            this.Bodies = bodies;
            this.gravitationalConstant = gravitationalConstant;
            this.shorteningLength = shorteningLength;

            //FMM vars
            this.MaxBodiesPerBox = MaxBodiesPerBox;


            //Initialise simulation vars
            bodyNum = Bodies.Length;

            positions = new double[bodyNum][];
            velocities = new double[bodyNum][];
            masses = new double[bodyNum];

            ForceEvals = new double[bodyNum][][];
            VelEvals = new double[bodyNum][][];
            posPredictions = new double[bodyNum][];

            //Preload the bodies and populate the arrays
            for (int i = 0; i < bodyNum; i++)
            {
                positions[i] = Bodies[i].Position;
                velocities[i] = Bodies[i].Velocity;
                masses[i] = Bodies[i].Mass;

                ForceEvals[i] = Bodies[i].ForceEvaluations;
                VelEvals[i] = Bodies[i].VelocityEvaluations;
            }


            //Generate initial points using RK4
            InitialiseRK4();
        }

        public Body[] getBodies()
        {
            //Push data back into Body objects
            for (int i = 0; i < bodyNum; i++)
            {
                Bodies[i].Position = positions[i];
                Bodies[i].Velocity = velocities[i];

                Bodies[i].ForceEvaluations = ForceEvals[i];
                Bodies[i].VelocityEvaluations = VelEvals[i];
            }

            return Bodies;
        }

        public void UpdateStep(int iters)
        {
           

            double[] displacement = new double[3];
            double[] force = new double[3];
            double[] forcePrediction = new double[3];
            double[] velocityPrediction = new double[3];
            double[] newVelocity = new double[3];
            double norm;

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
        }
        void InitialiseRK4()
        /*
         Creates the initial points using RK4
         */
        {
            double[,][] k = new double[bodyNum, 4][];
            double[,][] l = new double[bodyNum, 4][];

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
                for (int j = 0; j < bodyNum; j++)
                {
                    k[j, 1] = velocities[j].ScalarProduct(stepSize);

                    for (int j2 = 0; j2 < bodyNum; j2++)
                    {
                        if(j == j2) { continue; }
                        displacement = positions[j].Subtract(positions[j2]);
                        norm = displacement.Norm(shorteningLength);

                        l[j, 1] = l[j, 1].Subtract(displacement
                                                    .ScalarProduct(stepSize * gravitationalConstant * masses[j2]
                                                    / (norm * norm * norm)));
                    }
                    //Store g_n
                    ForceEvals[j].Push(l[j, 1]);
                    VelEvals[j].Push(velocities[j]);
                }

                //Find k2/l2 and k3/l3
                for (int i2 = 1; i2 <= 2; i2++)
                {
                    for (int j = 0; j < bodyNum; j++)
                    {
                        k[j, i2] = velocities[j].Add(l[j, i2-1].ScalarProduct(0.5)).ScalarProduct(stepSize);

                        for (int j2 = 0; j2 < bodyNum; j2++)
                        {
                            if (j == j2) { continue; }
                            displacement = positions[j].Add(k[j, i2-1].ScalarProduct(0.5))
                                .Subtract(positions[j2].Add(k[j2, i2-1].ScalarProduct(0.5)));
                            norm = displacement.Norm(shorteningLength);

                            l[j, i2] = l[j, i2].Subtract(displacement
                                                        .ScalarProduct(stepSize * gravitationalConstant * masses[j2]
                                                        / (norm * norm * norm)));
                        }
                    }
                }
                //Find k4/l4
                for (int j = 0; j < bodyNum; j++)
                {
                    k[j, 3] = velocities[j].Add(l[j, 2]).ScalarProduct(stepSize);

                    for (int j2 = 0; j2 < bodyNum; j2++)
                    {
                        if (j == j2) { continue; }
                        displacement = positions[j].Add(k[j, 2])
                            .Subtract(positions[j2].Add(k[j2, 2]));
                        norm = displacement.Norm(shorteningLength);

                        l[j, 3] = l[j, 3].Subtract(displacement
                                                    .ScalarProduct(stepSize * gravitationalConstant * masses[j2]
                                                    / (norm * norm * norm)));
                    }
                }

                //Update Position and Velocity vectors
                for (int j = 0; j < bodyNum; j++)
                {
                    positions[j] = positions[j].Add(k[j, 0]
                                                           .Add(k[j, 1].ScalarProduct(2.0))
                                                           .Add(k[j, 2].ScalarProduct(2.0))
                                                           .Add(k[j, 3]).ScalarProduct(1.0 / 6.0));

                    velocities[j] = velocities[j].Add(l[j, 0]
                                                           .Add(l[j, 1].ScalarProduct(2.0))
                                                           .Add(l[j, 2].ScalarProduct(2.0))
                                                           .Add(l[j, 3]).ScalarProduct(1.0 / 6.0));
                }
            }
        }

        public double GetEnergy()
            /*
                Returns kinetic + potential energy from the simulation's current state
             */
        {

            double energy = 0.0;
            double[] displacement = new double[3];
            double norm = 0.0;
            for (int i = 0; i < bodyNum; i++)
            {
                //Add kinetic energy
                energy += masses[i] * velocities[i].Norm()
                    * velocities[i].Norm() * 0.5;
                //Add potential energy
                for (int j = 0; j < bodyNum; j++)
                {
                    if (i == j) { continue; }

                    displacement = positions[i].Subtract(positions[j]);
                    

                    norm = displacement.Norm();
                    energy += (stepSize * gravitationalConstant * masses[i] * masses[j]
                                        / (norm));
                }
            }

            return energy;

        }
    }
}
