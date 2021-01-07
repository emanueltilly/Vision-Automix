using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision_Automix
{
    class ArrayTools
    {
        public static int[,] Fill2DArray(int[,] arr)
        {
            int numRows = arr.GetLength(0);
            int numCols = arr.GetLength(1);

            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numCols; ++j)
                {
                    arr[i, j] = 1;
                }
            }

            return arr;
        }

        public static int[] FillArray(int[] arr)
        {
            int counter = 0;
            foreach (int i in arr)
            {
                arr[counter] = 0;
                counter++;
            }
            return arr;
        }

        public static int GetIndexOfHighestValue(int[] array)
        {
            int indexOfLargest = 0;
            int largestValueFound = 0;

            int loopcounter = 0;
            foreach (int value in array)
            {
                if (value > largestValueFound) { largestValueFound = value; indexOfLargest = loopcounter; }
                loopcounter++;

            }

            return indexOfLargest;
        }
    }
}
