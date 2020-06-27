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
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = scalar * array[i];
            }
            return result;
        }

        public static double[] Product(this double[] array, double[] array2)
        {
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

        public static double[] Add(this double[] array, double[] array2)
        {
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
            if(array.Length != array2.Length)
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
        //Overload with optional shortening length
        public static double Norm(this double[] array, double shorteningLength)
        {
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
            double[][] result = new double[array.Length][];

            //Move all elements in the array to the right and insert the new value
            Array.Copy(array, 0, result, 1, (array.Length - 1));
            result[0] = element;


            return result;
        }
    }
}
