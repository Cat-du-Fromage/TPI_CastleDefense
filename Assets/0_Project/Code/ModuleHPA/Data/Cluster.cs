using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Mathematics;
using UnityEngine;

using static KWUtils.KWmath;
using static Unity.Mathematics.math;

namespace Kaizerwald
{
    public class Cluster// ou chunk
    {
        public readonly int ChunkIndex;

        public List<Gate> AllGates;
        public List<Gate>[] Gates;

        //==============================================================================================================
        //CONSTRUCTOR
        
        public Cluster(int index, in GridData gridData)
        {
            ChunkIndex = index;
            Gates = new List<Gate>[4];
            AllGates = new List<Gate>(4 * Cardinals.NumCardinals);
            CreateGates(gridData);
        }

        //==============================================================================================================
        //ACCESS METHODS
        
        public List<Gate> GetGatesAt(WaypointCardinal cardinal)
        {
            return Gates[(int)cardinal];
        }

        public Gate GetGateAt(WaypointCardinal cardinal, int index)
        {
            return Gates[(int)cardinal][index];
        }
        
        public int GetNumGatesAtCardinal(WaypointCardinal cardinal)
        {
            return Gates[(int)cardinal].Count;
        }

        //==============================================================================================================
        //INIT METHOD
        
        private void CreateGates(in GridData gridData)
        {
            for (int i = 0; i < Cardinals.NumCardinals; i++)
            {
                WaypointCardinal cardinal = (WaypointCardinal) i;
                
                Gates[i] = new List<Gate>(1) { InitializeGate(cardinal, gridData) };
                AllGates.AddRange(Gates[i]);
            }
        }
        
        private Gate InitializeGate(WaypointCardinal cardinal, in GridData gridData)
        {
            int gateIndex = GetCardinalFromIndex(cardinal, gridData.ChunkSize);
            return new Gate(ChunkIndex,gateIndex, cardinal, gridData);
        }
        
        private int GetCardinalFromIndex(WaypointCardinal cardinal, int chunkWidth)
        {
            int topRightCell = Sq(chunkWidth);
            int topLeftCell = topRightCell - chunkWidth - 1;
            
            int middleY = (topLeftCell / chunkWidth) / 2;
            int lastIndexInRow0 = chunkWidth - 1;

            return CardinalFromIndex();
            //=====================================================================
            //Private Methods
            
            int CardinalFromIndex() => cardinal switch
            {
                WaypointCardinal.North => (topRightCell + topLeftCell) / 2,
                WaypointCardinal.Est   => (middleY * chunkWidth) + lastIndexInRow0,
                WaypointCardinal.South => lastIndexInRow0 / 2,
                WaypointCardinal.West  => middleY * chunkWidth,
                _ => 0
            };
        }
    }
}
