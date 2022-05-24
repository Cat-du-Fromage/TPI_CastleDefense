using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Kaizerwald
{
    public readonly struct NativePath
    {
        public readonly Node Start;
        public readonly Node End;
        public readonly float Distance;

        public NativePath(Node start, Node end, float distance)
        {
            Start = start;
            End = end;
            Distance = distance;
        }
        
        public override string ToString()
        {
            return $"Start: {Start.Coord}; End: {End.Coord}; Distance:{Distance}";
        }
    }
    
    public class Path
    {
        public Node Start;
        public Node End;

        public int[] NodesPath;
        public float Distance;

        public Path(Node start, Node end, NativeList<int> path)
        {
            Start = start;
            End = end;
            NodesPath = path.ToArray();
            Distance = NodesPath.Length;
        }
        
        public Path(Node start, Node end, List<int> path)
        {
            Start = start;
            End = end;
            NodesPath = path.ToArray();
            Distance = NodesPath.Length;
        }

        public static implicit operator NativePath(Path path)
        {
            return new NativePath(path.Start, path.End, path.Distance);
        }
        
        public override string ToString()
        {
            return $"Start: {Start.Coord}; End: {End.Coord}; Distance:{Distance}";
        }
    }

    public struct JTestPath : IJob
    {
        private NativePath nativePath;

        public JTestPath(Path path)
        {
            nativePath = path;
        }
        
        public void Execute()
        {
            //Debug.Log(nativePath.ToString());
        }
    }
}
