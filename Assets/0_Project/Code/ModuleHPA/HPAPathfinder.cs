using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

using static KWUtils.KWGrid;
using static Unity.Mathematics.math;

namespace Kaizerwald
{
    public partial class HPAPathfinder : MonoBehaviour
    {
        private HPAGrid hpaGrid;
        
        [SerializeField] private Transform StartObject;
        [SerializeField] private Transform EndObject;
        
        //On doit avoir en premier lieu : Clusters concerné
        private int clusterStartIndex;
        private int clusterEndIndex;

        private int2 startCellCoord;
        private int2 endCellCoord;
        
        //List node Index par ordre de distance
        private DistanceIndex distanceGridIndex;
        private List<DistanceIndex> startGatesByDistance;
        private List<DistanceIndex> endGatesByDistance;

        private int[] startPath;
        private int[] endPath;

        private void Awake()
        {
            hpaGrid = GetComponent<HPAGrid>();
        }

        private void Start()
        {
            if (StartObject == null || EndObject == null) return;
            GetClustersStartEnd();
            FindShortestGates();
        }

        private void GetClustersStartEnd()
        {
            int startGridIndex = StartObject.position.GetIndexFromPosition(hpaGrid.MapSize, 1);
            clusterStartIndex = hpaGrid.ChunkedGrid.ChunkIndexFromGridIndex(startGridIndex);
            startCellCoord = startGridIndex.GetXY2(hpaGrid.MapSize.x);
            
            int endGridIndex = EndObject.position.GetIndexFromPosition(hpaGrid.MapSize, 1);
            clusterEndIndex = hpaGrid.ChunkedGrid.ChunkIndexFromGridIndex(endGridIndex);
            endCellCoord = endGridIndex.GetXY2(hpaGrid.MapSize.x);
        }
        
        //1 Trouver porte la plus proche du start;
        private void FindShortestGates()
        {
            //start shortest
            ShortestGate(ref startGatesByDistance, clusterStartIndex, endCellCoord);
            //start shortest
            ShortestGate(ref endGatesByDistance, clusterEndIndex, startCellCoord);
        }
        
        //==============================================================================================================
        //GET SHORTEST START END GATE
        private void ShortestGate(ref List<DistanceIndex> distances, int fromIndex, in int2 toCoord)
        {
            int numGates = hpaGrid.Clusters[fromIndex].AllGates.Count;
            distances ??= new List<DistanceIndex>(numGates);
            distances.Clear();
            for (int i = 0; i < numGates; i++)
            {
                int2 coord = hpaGrid.Clusters[fromIndex].AllGates[i].GridIndex.GetXY2(hpaGrid.MapSize.x);
                int numCellDistance = CalculateDistanceCost(coord, toCoord); //csum(abs(toCoord - coord));

                distanceGridIndex = new DistanceIndex(fromIndex, numCellDistance, hpaGrid.Clusters[fromIndex].AllGates[i].GridIndex);
                distances.Add(distanceGridIndex);
            }
            distances.Sort();
            
            int CalculateDistanceCost(in int2 a, in int2 b)
            {
                int2 xyDistance = abs(a - b);
                int remaining = abs(xyDistance.x - xyDistance.y);
                return 14 * cmin(xyDistance) + 10 * remaining;
            }
        }
        //==============================================================================================================
        
        //==============================================================================================================
        //GET: PATH: START-GATE
        
        
        
        //==============================================================================================================
    }
}
