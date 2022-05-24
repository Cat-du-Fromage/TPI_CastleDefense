using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using UnityEngine;

namespace Kaizerwald
{
    
    public class Gate
    {
        public readonly WaypointCardinal Cardinal;
        
        public readonly int ChunkIndex;
        public readonly int IndexInChunk;
        public readonly int GridIndex;

        public List<Path> PathsToGates; // we may have multiple gate per cardinal

        public Gate(int chunkIndex, int gridIndex, WaypointCardinal waypointCardinal, in GridData gridData)
        {
            Cardinal = waypointCardinal;
            
            ChunkIndex = chunkIndex;
            IndexInChunk = gridIndex;
            GridIndex = ChunkIndex.GetGridCellIndexFromChunkCellIndex(gridData, gridIndex);
            PathsToGates = new List<Path>(4); //4 because there might be an other gate on the same line
        }

        public NativeArray<NativePath> ToNativePaths()
        {
            NativeArray<NativePath> nativePaths = new (PathsToGates.Count, Allocator.TempJob);
            for (int i = 0; i < PathsToGates.Count; i++) nativePaths[i] = PathsToGates[i];
            return nativePaths;
        }

        public override string ToString()
        {
            return $"chunkIndex: {ChunkIndex}; indexInChunk: {IndexInChunk}; numPath:{PathsToGates?.Count}";
        }
    }
}
