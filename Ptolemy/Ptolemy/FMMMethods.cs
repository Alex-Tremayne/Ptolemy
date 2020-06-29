using System;
using System.Collections.Generic;
using System.Text;

namespace Ptolemy
{
    class FMMMethods
    {
        //Effect of further multipoles can probably be reduced, 
        //larger timesteps for long distance interactions with timesteps equal to some multiple of the smaller scale interaction timestep
        //Using adaptive code this could probably achieve stable error 

        public static uint GetMortonIndex(uint[] CellCoords)
        {
            //Obtain the morton index from the coordinates of a cell
            for (int i = 0; i < 3; i++)
            {
                CellCoords[i] &= 0x000003ff;
                CellCoords[i] = (CellCoords[i] | (CellCoords[i] << 16)) & 0xff0000ff;
                CellCoords[i] = (CellCoords[i] | (CellCoords[i] << 8)) & 0x0300f00f;
                CellCoords[i] = (CellCoords[i] | (CellCoords[i] << 4)) & 0x030c30c3;
                CellCoords[i] = (CellCoords[i] | (CellCoords[i] << 2)) & 0x09249249;
            }



            return CellCoords[0] | (CellCoords[1] << 1) | (CellCoords[2] << 2);
        }

        public static uint[] GetCellCoords(uint index)
        {
            //Obtain the coordinates of a cell from the morton index
            uint[] CellCoords = new uint[3];

            uint temp;
            for (int i = 0; i < 3; i++)
            {
                temp = (index >> i) & 0x09249249;
                temp = (temp | (temp >> 2)) & 0x030c30c3;
                temp = (temp | (temp >> 4)) & 0x0300f00f;
                temp = (temp | (temp >> 8)) & 0xff0000ff;
                CellCoords[i] = (temp | (temp >> 16)) & 0x000003ff;
            }

            return CellCoords;
        }

        public static uint[] GetNeighbours(uint index)
        {
            //I really hate this algorithm, it needs to be replaced but it does work, for now

            //Will do edge detection only for upper-left-lower edges, would need division level for
            //other edges

            uint[] CellCoords = GetCellCoords(index);
            uint x, y, z;

            int skips = 1;//Must skip centre

            //This is a dumb way of checking how many of them are actually 0
            //But it does work, and technically it doesn't care which ones are 0
            //But it's probably slower than just having a bunch of if statements for each coord
            if(CellCoords[0]*CellCoords[1]*CellCoords[2] == 0)
            {
                if(CellCoords[0]*CellCoords[1]+CellCoords[0]*CellCoords[2]+CellCoords[1]*CellCoords[2] == 0)
                {
                    if(CellCoords[0]+CellCoords[1]+CellCoords[2] == 0)
                    {
                        //Skip the 19 boxes on the sides plus the centre
                        skips = 20;
                    }
                    else
                    {
                        //Skip the 15 boxes on the sides plus the centre
                        skips = 16;
                    }
                } else
                {
                    //Skip the nine boxes on one side plus the centre
                    skips = 10;
                }
            }

            uint[][] NeighbourCoords = new uint[27 - skips][];
            uint[] NeighbourIndices = new uint[27 - skips];
            int counter = 0;

            for (uint i = 0; i < 27; i++)
            {

                z = i / 9;
                y = (i - 9 * z) / 3;
                x = (i - 9 * z - 3 * y);

                if(CellCoords[0] + x == 0 || CellCoords[1] + y == 0 || CellCoords[2] + z == 0 || x*y*z == 1)
                {
                    //Skip any negative coordinates
                    //Might add skipping based on level of divisions
                    continue;
                }

                NeighbourCoords[counter] = new uint[]
                        { CellCoords[0] + x - 1, CellCoords[1] + y - 1, CellCoords[2] + z - 1 };

                counter++;
            }

            for(int i = 0; i < 27 - skips; i++)
            {
                NeighbourIndices[i] = GetMortonIndex(NeighbourCoords[i]);
            }

            return NeighbourIndices;
        }

        public static int[] DecomposeDomain(ref Body[] Bodies, double[] Boundary, int level = -1, bool sort = true)
            //Optional level var, if non-negative, decompose directly at that level
            //Domain is sorted unless sort = false
        {
            if(level >= 0)
            {
                int divisions = 2 * level;
                uint[] CellCoords = new uint[3]; 

                foreach(Body body in Bodies)
                {
                    CellCoords = body.Position.Quotient(Boundary).ScalarProduct(divisions).ToUint();
                    body.MortonIndex = GetMortonIndex(CellCoords);
                }
            } else
            {
                //auto choose level
            }
            if(sort)
            {
                Array.Sort(Bodies,
                    delegate (Body x, Body y) { return x.MortonIndex.CompareTo(y.MortonIndex); });
            }

            int[] OffSets = new int[] { };
            int indexCurrent = -1;
            int NCell = 0;
            
            for(int i = 1; i < Bodies.Length; i++)
            {
                if(indexCurrent != Bodies[i].MortonIndex)
                {
                    indexCurrent = (int)Bodies[i].MortonIndex;
                    OffSets[NCell] = i;
                    NCell++;

                }
            }

            return OffSets;
        }
    }
}
