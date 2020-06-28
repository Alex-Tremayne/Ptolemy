using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ptolemy
{
    static class ArrayExtensions
    {
        public static double[] ScalarProduct(this double[] array, double scalar)
        {
            //Multiply each element by a scalar
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = scalar * array[i];
            }
            return result;
        }

        public static double[] Product(this double[] array, double[] array2)
        {
            //Elementwise multiply
            if (array.Length != array2.Length)
            {
                return null;
            }
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i] * array2[i];
            }
            return result;
        }

        public static double[] Quotient(this double[] array, double[] array2)
        {
            //Elementwise divide
            if (array.Length != array2.Length)
            {
                return null;
            }
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i] / array2[i];
            }
            return result;
        }

        public static double[] Add(this double[] array, double[] array2)
        {
            //Elementwise addition
            if (array.Length != array2.Length)
            {
                return null;
            }
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i] + array2[i];
            }
            return result;
        }

        public static double[] Subtract(this double[] array, double[] array2)
        {
            //Elementwise subtraction
            if (array.Length != array2.Length)
            {
                return null;
            }
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i] - array2[i];
            }
            return result;
        }

        public static double Norm(this double[] array)
        {
            double result = 0.0;
            foreach (double element in array)
            {
                result += element * element;
            }
            return Math.Sqrt(result);
        }
        
        public static double Norm(this double[] array, double shorteningLength)
        {
            //Overload with optional shortening length

            double result = 0.0;
            foreach (double element in array)
            {
                result += element * element;
            }
            return Math.Sqrt(result + shorteningLength * shorteningLength);
        }

        public static double[] Push(this double[] array, double element)
        {
            double[] result = new double[array.Length];

            //Move all elements in the array to the right and insert the new value
            Array.Copy(array, 0, result, 1, (array.Length - 1));
            result[0] = element;


            return result;
        }

        public static double[][] Push(this double[][] array, double[] element)
        {
            //Overload for inserting into jagged array

            double[][] result = new double[array.Length][];

            //Move all elements in the array to the right and insert the new value
            Array.Copy(array, 0, result, 1, (array.Length - 1));
            result[0] = element;


            return result;
        }

        public static uint[] ToUint(this double[] array)
        {
            //Overload for inserting into jagged array

            uint[] result = new uint[array.Length];

            for(int i = 0; i < array.Length; i++)
            {
                result[i] = (uint)array[i];
            }


            return result;
        }
    }
}
