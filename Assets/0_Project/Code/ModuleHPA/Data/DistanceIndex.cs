using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaizerwald
{
    public readonly struct DistanceIndex: IComparable<DistanceIndex>
    {
        public readonly int ChunkIndex;
        public readonly int NumCellDistance;
        public readonly int CellGridIndex;

        public DistanceIndex(int chunkIndex, int distance, int fromIndex)
        {
            ChunkIndex = chunkIndex;
            NumCellDistance = distance;
            CellGridIndex = fromIndex;
        }

        public int CompareTo(DistanceIndex other)
        {
            return NumCellDistance.CompareTo(other.NumCellDistance);
        }
    }
}
