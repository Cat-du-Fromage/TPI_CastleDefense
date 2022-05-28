using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KWUtils;
using KWUtils.KWGenericGrid;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static KWUtils.KWmath;

namespace KaizerWald
{
    public partial class HPAGrid : MonoBehaviour
    {
        public List<Cluster> Clusters;

        public int2 MapSize { get; private set; } = new int2(128);
        public int ChunkSize { get; private set; } = 32;

        private List<int[]> pathBetweenGates;
        
        public GenericChunkedGrid<Node> ChunkedGrid { get; private set; }

        private NativeArray<bool> obstacles;
        private NativeArray<Node> nodes;
        private NativeList<int> path;

        private void Awake()
        {
            ChunkedGrid = new GenericChunkedGrid<Node>(MapSize, ChunkSize, 1,(index) => new Node(index.GetXY2(ChunkSize)));
            CreateCluster();
            GetPathsBetweenChunks();
        }

        private void CreateCluster()
        {
            Clusters = new List<Cluster>(ChunkedGrid.ChunkDictionary.Count);
            for (int i = 0; i < ChunkedGrid.ChunkDictionary.Count; i++)
            {
                Clusters.Add(new Cluster(i, ChunkedGrid.GridData));
            }
            //TestGateEntry = true;
        }

        private void GetPathsBetweenChunks()
        {
            for (int i = 0; i < Clusters.Count; i++)
            {
                for (int j = 0; j < Cardinals.NumCardinals; j++)
                {
                    WaypointCardinal cardinal = (WaypointCardinal)j;
                    int gatePerBorder = Clusters[i].GetNumGatesAtCardinal(cardinal);
                    
                    for (int k = 0; k < gatePerBorder; k++)
                    {
                        Gate currentGate = Clusters[i].GetGateAt(cardinal, k);
                        GetPaths(Clusters[i], currentGate);
                    }
                }
            }
        }

        private void GetPaths(Cluster cluster, Gate gate)
        {
            gate.PathsToGates = new List<Path>(cluster.AllGates.Count);
            
            obstacles = new NativeArray<bool>(ChunkSize * ChunkSize, Allocator.TempJob);
            path = new NativeList<int>(ChunkSize / 2, Allocator.TempJob);

            //-1 because we dont count "gate" we currently check paths
            for (int j = 0; j < cluster.AllGates.Count; j++)
            {
                if (cluster.AllGates[j] == gate) continue;
                Node startNode = ChunkedGrid.GetValues(gate.ChunkIndex)[gate.IndexInChunk];
                
                int endIndex = cluster.AllGates[j].IndexInChunk;
                Node endNode = ChunkedGrid.GetValues(gate.ChunkIndex)[endIndex];
                
                bool pathFound = GetPathToGate(gate.ChunkIndex, gate.IndexInChunk, endIndex);
                if (pathFound)
                {
                    Path p = new Path(startNode, endNode, path);
                    JTestPath testjob = new JTestPath(p);
                    testjob.Schedule().Complete();
                    gate.PathsToGates.Add(p);
                }
                path.Clear();
            }
            obstacles.Dispose();
            path.Dispose();
        }

        private bool GetPathToGate(int chunkIndex, int startIndex, int endIndex)
        {
            //CAREFULL NODES MUST BE RESET!
            using (nodes = ChunkedGrid.ChunkDictionary[chunkIndex].ToNativeArray())
            {
                JaStar job = new JaStar
                {
                    NumCellX = ChunkSize,
                    StartNodeIndex = startIndex, //get we start on
                    EndNodeIndex = endIndex, //need all 3 other gates
                    ObstaclesGrid = obstacles,
                    Nodes = nodes,
                    PathList = path
                };
                job.Schedule().Complete();
                return !path.IsEmpty;
            }
        }
    }
}
