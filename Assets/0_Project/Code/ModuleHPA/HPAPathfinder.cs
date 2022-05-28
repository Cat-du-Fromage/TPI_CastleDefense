using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

using static KWUtils.KWGrid;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace KaizerWald
{
    public partial class HPAPathfinder : MonoBehaviour
    {
        private int2 mapSize;
        private IHighlightCoordinator coordinator;
        private HPAGrid hpaGrid;

        public Dictionary<int, int> currentTargetWaypoint { get; private set; }  = new (2);

        private Dictionary<int, Vector3> StartRegiment = new (2);
        private Dictionary<int, Gate> StartGateRegiment = new (2);
        private Dictionary<int, Gate> EndGateRegiment = new (2);
        public Dictionary<int, Tuple<Vector3, Quaternion>> DestinationRegiment { get; private set; } = new (2);
        
        private Dictionary<int, List<DistanceIndex>> startGatesByDistance = new (2);
        private Dictionary<int, List<DistanceIndex>> endGatesByDistance = new (2);
        
        //List node Index par ordre de distance
        private DistanceIndex distanceGridIndex;

        private int[] startPath;
        private int[] endPath;
        
        public Dictionary<int, List<Vector3>> Waypoints { get; private set; } = new (2);
        //private Dictionary<int, List<int>> endGatesByDistance = new Dictionary<int, List<int>>(2);

        private void Awake()
        {
            //==============================================================================================================
            //CONNECTION : HIGHLIGHT COORDINATOR
            RegimentManager manager = FindObjectOfType<RegimentManager>();
            coordinator = (IHighlightCoordinator)manager;
            //==============================================================================================================
            
            hpaGrid = GetComponent<HPAGrid>();
        }

        private void Start()
        {
            mapSize = hpaGrid.MapSize;

            if (defaultDebugState)
            {
                ShowPaths = ShowEndStart = ShowMovingRegiment = ShowStartEndGate = true;
            }
        }

        public void RegisterStart(Regiment regiment)
        {
            //Start PART : Need to Reposition the regiment
            (Vector3 posStart, Quaternion rotStart) = GetLeaderStartPositionAndRotation();
            regiment.transform.SetPositionAndRotation(posStart, rotStart);
            
            if (!StartRegiment.TryAdd(regiment.RegimentID, posStart))
            {
                StartRegiment[regiment.RegimentID] = posStart;
            }
            
            //End PART : Do not Reposition Regiment
            Tuple<Vector3, Quaternion> posRotEnd = GetLeaderEndPositionAndRotation();

            if (!DestinationRegiment.TryAdd(regiment.RegimentID, posRotEnd))
            {
                DestinationRegiment[regiment.RegimentID] = posRotEnd;
            }
            
            FindShortestGates(regiment);

            Test(regiment);

            if (!currentTargetWaypoint.TryGetValue(regiment.RegimentID, out _))
            {
                currentTargetWaypoint.Add(regiment.RegimentID, 0);
            }
            else
            {
                currentTargetWaypoint[regiment.RegimentID] = 0;
            }
            
            //==========================================================================================================
            // INTERNAL METHODS
            //==========================================================================================================
            //START DESTINATION
            Tuple<Vector3, Quaternion> GetLeaderEndPositionAndRotation()
            {
                int indexLastUnitFirstLine = regiment.CurrentLineFormation - 1;
                IHighlightRegister register = coordinator.PlacementSystem.Register;
                
                Vector3 firstUnitFirstLine = register.Records[regiment.RegimentID][0].HighlightTransform.position;
                Vector3 lastUnitFirstLine = register.Records[regiment.RegimentID][indexLastUnitFirstLine].HighlightTransform.position;
                
                Vector3 direction = firstUnitFirstLine.DirectionTo(lastUnitFirstLine);
                Vector3 crossDirection = Vector3.Cross(direction, Vector3.up);
                
                Quaternion rotation = quaternion.LookRotationSafe(crossDirection, up());

                return new Tuple<Vector3, Quaternion>((firstUnitFirstLine + lastUnitFirstLine) / 2f, rotation);
            }

            //END DESTINATION
            Tuple<Vector3, Quaternion> GetLeaderStartPositionAndRotation()
            {
                int indexLastUnitFirstLine = regiment.CurrentLineFormation - 1;
                Vector3 firstUnitFirstLine = regiment.Units[0].transform.position;
                Vector3 lastUnitFirstLine = regiment.Units[indexLastUnitFirstLine].transform.position;

                Vector3 direction = firstUnitFirstLine.DirectionTo(lastUnitFirstLine);
                Vector3 crossDirection = Vector3.Cross(direction, Vector3.up);
                
                Quaternion rotation = quaternion.LookRotationSafe(crossDirection, up());
                
                return new Tuple<Vector3, Quaternion>((firstUnitFirstLine + lastUnitFirstLine) / 2f, rotation);
            }
            //==========================================================================================================
        }

        //1 Trouver porte la plus proche du start;
        private void FindShortestGates(Regiment regiment)
        {
            //start shortest
            if (StartRegiment.TryGetValue(regiment.RegimentID, out Vector3 positionStart)
                && DestinationRegiment.TryGetValue(regiment.RegimentID, out Tuple<Vector3, Quaternion> poRotStart))
            {
                int startGridIndex = positionStart.GetIndexFromPosition(hpaGrid.MapSize, 1);
                int clusterStartIndex = hpaGrid.ChunkedGrid.ChunkIndexFromGridIndex(startGridIndex);
                //int2 startCellCoord = startGridIndex.GetXY2(hpaGrid.MapSize.x);
                
                //End shortest
                int endGridIndex = poRotStart.Item1.GetIndexFromPosition(hpaGrid.MapSize, 1);
                int clusterEndIndex = hpaGrid.ChunkedGrid.ChunkIndexFromGridIndex(endGridIndex);
                int2 endCellCoord = endGridIndex.GetXY2(hpaGrid.MapSize.x);
                
                //End shortest
                ShortestGate(ref startGatesByDistance, regiment.RegimentID,clusterStartIndex, endCellCoord, true);
                
                //start GATE coord
                int startGateIndex = startGatesByDistance[regiment.RegimentID][0].CellGridIndex;
                //int clusterStartGate = startGatesByDistance[regiment.RegimentID][0].ChunkIndex;
                int2 startGateCoord = startGateIndex.GetXY2(hpaGrid.MapSize.x);
                
                
                //int2 startGateCoord = hpaGrid.ChunkedGrid.GetValues()  StartRegiment[regiment.RegimentID][0]
                ShortestGate(ref endGatesByDistance, regiment.RegimentID,clusterEndIndex, startGateCoord, false);
            }
        }
        
        //==============================================================================================================
        //GET SHORTEST START END GATE
        private void ShortestGate(ref Dictionary<int, List<DistanceIndex>> distances, int regimentIndex, int fromIndex, in int2 toCoord, bool start)
        {
            int numGates = hpaGrid.Clusters[fromIndex].AllGates.Count;
            distances ??= new Dictionary<int, List<DistanceIndex>>(numGates);
            
            //Check if need to add key or just clear value at regiment Key
            if (!distances.TryGetValue(regimentIndex, out _))
            {
                distances.Add(regimentIndex, new List<DistanceIndex>(numGates));
            }
            else
            {
                distances[regimentIndex].Clear();
            }

            for (int i = 0; i < numGates; i++)
            {
                int2 coord = hpaGrid.Clusters[fromIndex].AllGates[i].GridIndex.GetXY2(hpaGrid.MapSize.x);
                int numCellDistance = CalculateDistanceCost(coord, toCoord); //csum(abs(toCoord - coord));

                distanceGridIndex = new DistanceIndex(fromIndex, numCellDistance, hpaGrid.Clusters[fromIndex].AllGates[i].GridIndex);

                distances[regimentIndex].Add(distanceGridIndex);
            }
            distances[regimentIndex].Sort();

            //STORE GATE FOR LATER PATHFINDING
            if (start)
                StoreStartGate(regimentIndex, fromIndex, distances[regimentIndex][0].CellGridIndex);
            else
                StoreEndGate(regimentIndex, fromIndex, distances[regimentIndex][0].CellGridIndex);
        }
        
        private int CalculateDistanceCost(in int2 a, in int2 b)
        {
            int2 xyDistance = abs(a - b);
            int remaining = abs(xyDistance.x - xyDistance.y);
            return 14 * cmin(xyDistance) + 10 * remaining;
        }
        //==============================================================================================================

        private void StoreStartGate(int regimentIndex, int clusterIndex, int gateIndex)
        {
            if (!StartGateRegiment.TryGetValue(regimentIndex, out Gate _))
                StartGateRegiment.Add(regimentIndex, GetGate(clusterIndex, gateIndex));
            else
                StartGateRegiment[regimentIndex] = GetGate(clusterIndex, gateIndex);
        }
        
        private void StoreEndGate(int regimentIndex, int clusterIndex, int gateIndex)
        {
            if (!EndGateRegiment.TryGetValue(regimentIndex, out Gate _))
                EndGateRegiment.Add(regimentIndex, GetGate(clusterIndex, gateIndex));
            else
                EndGateRegiment[regimentIndex] = GetGate(clusterIndex, gateIndex);
        }
        
        private Gate GetGate(int clusterIndex, int gateIndex)
        {
            int numGates = hpaGrid.Clusters[clusterIndex].AllGates.Count;
            for (int i = 0; i < numGates; i++)
            {
                if (hpaGrid.Clusters[clusterIndex].AllGates[i].GridIndex != gateIndex) continue;
                return hpaGrid.Clusters[clusterIndex].AllGates[i];
            }
            return hpaGrid.Clusters[clusterIndex].AllGates[0];
        }
        
        //==============================================================================================================
        //GET: PATH: START-GATE
        private void Test(Regiment regiment)
        {
            //Path: RegimentStart -> startGate
            //waypoints
            int regimentId = regiment.RegimentID;
            CheckNullWaypoints(regimentId);
            
            Waypoints[regimentId].Clear();
            Waypoints[regimentId] = GetPathList(regimentId);
            Waypoints[regimentId].Add(DestinationRegiment[regimentId].Item1);
        }

        private List<Vector3> GetPathList(int regimentIndex)
        {
            int numChunkX = hpaGrid.ChunkedGrid.GridData.NumChunkXY.x;
            
            Gate currentGate;
            
            //=========================================================
            //END Adjacent GATE
            currentGate = EndGateRegiment[regimentIndex];
            //(int adjEndCluster, int adjEndGateIndex) = GetAdjacentClusterAndGate(numChunkX);
            Gate endGate = currentGate;
            int2 endGateCoord = endGate.GetCellCoord(mapSize.x);
            //=========================================================
            
            List<Vector3> paths = new List<Vector3>(2);
            currentGate = StartGateRegiment[regimentIndex];
            paths.Add(currentGate.CellPosition(mapSize));
            
            NativeList<DistanceIndex> distances = new NativeList<DistanceIndex>(4, Allocator.Temp);
            while (currentGate != EndGateRegiment[regimentIndex])
            {
                
                (int adjCluster, int adjGateIndex) = GetAdjacentClusterAndGate(numChunkX);
                //Adjacent Gate
                currentGate = GetGate(adjCluster, adjGateIndex);
                paths.Add(currentGate.CellPosition(mapSize));
                
                if (endGate.ChunkIndex == currentGate.ChunkIndex)
                {
                    //paths.Add(endGate.CellPosition(mapSize));
                    break;
                }

                int gateChunkIndex = currentGate.ChunkIndex;
                int numGates = hpaGrid.Clusters[gateChunkIndex].AllGates.Count;

                for (int i = 0; i < numGates; i++)
                {
                    //Dont compare SAME GATE!
                    if (hpaGrid.Clusters[gateChunkIndex].AllGates[i].GridIndex == currentGate.GridIndex) continue;
                    
                    //Get Dst btwn gate "i" and end Gate
                    int2 coord = hpaGrid.Clusters[gateChunkIndex].AllGates[i].GridIndex.GetXY2(hpaGrid.MapSize.x);
                    int numCellDistance = CalculateDistanceCost(coord, endGateCoord);

                    distanceGridIndex = new DistanceIndex(gateChunkIndex, numCellDistance, hpaGrid.Clusters[gateChunkIndex].AllGates[i].GridIndex);

                    distances.Add(distanceGridIndex);
                    //distances[regimentIndex].Add(distanceGridIndex);
                }
                distances.Sort();
                currentGate = GetGate(distances[0].ChunkIndex, distances[0].CellGridIndex);
                paths.Add(currentGate.CellPosition(mapSize));
                
                
                
                distances.Clear();
            }

            distances.Dispose();
            return paths;

            (int, int) GetAdjacentClusterAndGate(int chunkX) => currentGate.Cardinal switch
            {
                WaypointCardinal.North => (currentGate.ChunkIndex + chunkX, currentGate.GridIndex + mapSize.x),
                WaypointCardinal.Est   => (currentGate.ChunkIndex + 1, currentGate.GridIndex + 1),
                WaypointCardinal.South => (currentGate.ChunkIndex - chunkX, currentGate.GridIndex - mapSize.x),
                WaypointCardinal.West  => (currentGate.ChunkIndex - 1, currentGate.GridIndex - 1),
                _ => (-1, -1)
            };
            
        }

        private void CheckNullWaypoints(int index)
        {
            if (!Waypoints.TryGetValue(index, out _))
                Waypoints.Add(index, new List<Vector3>(2));
            else if(Waypoints[index] == null)
                Waypoints[index] = new List<Vector3>(2);
        }
        //==============================================================================================================
    }
}
