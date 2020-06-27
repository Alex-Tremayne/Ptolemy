using System;
using System.Collections.Generic;
using System.Text;

namespace Ptolemy
{
    class FMMMethods
    {
        public static uint GetMortonIndex(uint[] boxCoords)
        {
            //Obtain the morton index from the coordinates of a cell
            for (int i = 0; i < 3; i++)
            {
                boxCoords[i] &= 0x000003ff;
                boxCoords[i] = (boxCoords[i] | (boxCoords[i] << 16)) & 0xff0000ff;
                boxCoords[i] = (boxCoords[i] | (boxCoords[i] << 8)) & 0x0300f00f;
                boxCoords[i] = (boxCoords[i] | (boxCoords[i] << 4)) & 0x030c30c3;
                boxCoords[i] = (boxCoords[i] | (boxCoords[i] << 2)) & 0x09249249;
            }



            return boxCoords[0] | (boxCoords[1] << 1) | (boxCoords[2] << 2);
        }

        public static uint[] GetBoxCoords(uint index)
        {
            //Obtain the coordinates of a cell from the morton index
            uint[] boxCoords = new uint[3];

            uint temp;
            for (int i = 0; i < 3; i++)
            {
                temp = (index >> i) & 0x09249249;
                temp = (temp | (temp >> 2)) & 0x030c30c3;
                temp = (temp | (temp >> 4)) & 0x0300f00f;
                temp = (temp | (temp >> 8)) & 0xff0000ff;
                boxCoords[i] = (temp | (temp >> 16)) & 0x000003ff;
            }

            return boxCoords;
        }

        public static void InsertionSort(int[] KeyArray, int[] OffSets)
        {
            //I don't think this is necessarily an insertion sort, but it'll probably do some inserting 
            //Frankly this whole method is a bit disgusting
            int[][] swaps = new int[][] { };
            int swapsCounter = 0;

            int setIndex = 0;

            int[] NewOffsets = OffSets;
            for(int i = 0; i < KeyArray.Length; i++)
            {
                while(i == OffSets[setIndex + 1])
                {
                    //Here we assume that empty cells are indexed to the start of the next non-empty cell
                    setIndex++;
                }

                if(KeyArray[i] != setIndex)
                {
                    if(KeyArray[i] > setIndex)
                    {
                        //Move from location i to the start of the correct set
                        swaps[swapsCounter] = new int[] { i, NewOffsets[setIndex] - 1 };

                        for (int j = setIndex; j < KeyArray[i]; j++)
                        {
                            //Update the newoffsets in the range
                            //Set it is moving from stays the same
                            //Set it is moving to decreases by 1
                            NewOffsets[j + 1] -= 1;
                        }
                    } else {
                        //Move from location i to the start of the correct set
                        swaps[swapsCounter] = new int[] { i, NewOffsets[setIndex] };

                        for (int j = KeyArray[i]; j < setIndex; j++)
                        {
                            //Update the newoffsets in the range
                            //Set it is moving from increases by 1
                            //Set it is moving to stays the same
                            NewOffsets[j + 1] += 1;
                        }
                    }
                }
            }

            //TO-DO sort the array and return everything

        }
    }
}
