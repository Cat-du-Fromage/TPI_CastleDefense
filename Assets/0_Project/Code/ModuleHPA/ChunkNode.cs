using System;
using System.Collections.Generic;
using KWUtils;

namespace KaizerWald
{
    public struct ChunkNode
    {
        private int[] waypoints;
        private List<int>[] gates;

        public int GetWaypoints(WaypointCardinal cardinal)
        {
            return waypoints[(byte)cardinal];
        }
        
        
    }
}