using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Kaizerwald
{
    public static class Utils
    {
        public static int ChunkIndexFromGridIndex(int gridIndex, in GridData gridData )
        {
            int2 cellCoord = gridIndex.GetXY2(gridData.MapSize.x);
            int2 chunkCoord = (int2)floor(cellCoord / gridData.ChunkSize);
            return chunkCoord.GetIndex(gridData.NumChunkXY.x);
        }
        
        public static int CellChunkIndexFromGridIndex(int gridIndex, in GridData gridData)
        {
            int2 cellCoord = gridIndex.GetXY2(gridData.MapSize.x);
            int2 chunkCoord = (int2)floor(cellCoord / gridData.ChunkSize);
            int2 cellCoordInChunk = cellCoord - (chunkCoord * gridData.ChunkSize);
            return cellCoordInChunk.GetIndex(gridData.ChunkSize);
        }

        private static int2 GetCoord(this int i, int width)
        {
            int y = i / width;
            int x = i - (y * width);
            return new int2(x, y);
        }
    }
}
