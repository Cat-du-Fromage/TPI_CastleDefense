using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
namespace KaizerWald
{
    public partial class HPAPathfinder
    {
        private bool defaultDebugState = true;

        public bool ShowMovingRegiment;
        public bool ShowPaths;
        public bool ShowEndStart;
        public bool ShowStartEndGate;

        private Vector3 startCellPosition = Vector3.zero;
        private Vector3 endCellPosition = Vector3.zero;
        
        private void OnDrawGizmos()
        {
            DrawStartEnd();
            ShowStartEndGates();
            Debug_PathsInGame();
            Debug_MovingRegiment();
        }

        private void Debug_MovingRegiment()
        {
            if (!ShowMovingRegiment) return;
            if (coordinator.MovingRegiments.Count == 0) return;
            Vector3 rectangleSize = new Vector3(1, 3, 1);
            foreach (Regiment regiment in coordinator.MovingRegiments)
            {
                Gizmos.DrawCube(regiment.transform.position, rectangleSize);
            }
        }

        private void DrawStartEnd()
        {
            if (!ShowEndStart) return;
            if (StartRegiment.Count == 0 || DestinationRegiment.Count == 0) return;
            float size = 0.4f;

            Gizmos.color = Color.green;
            foreach ((int i, Vector3 pos) in StartRegiment)
            {
                startCellPosition = GetCellPosition(pos);
                Gizmos.DrawSphere(startCellPosition, size);
            }
            
            Gizmos.color = Color.red;
            foreach ((int i, (Vector3 pos, Quaternion q)) in DestinationRegiment)
            {
                endCellPosition = GetCellPosition(pos);
                Gizmos.DrawSphere(endCellPosition, size);
            }
        }

        private Vector3 GetCellPosition(Vector3 pos)
        {
            int beginGridIndex = pos.GetIndexFromPosition(hpaGrid.MapSize, 1);
            int beginIndexInChunk = hpaGrid.ChunkedGrid.CellChunkIndexFromGridIndex(beginGridIndex);
            
            int clusterBeginId = hpaGrid.ChunkedGrid.ChunkIndexFromGridIndex(beginGridIndex);
            //int2 beginCellCoord = sGridIndex.GetXY2(hpaGrid.MapSize.x);
            
            return hpaGrid.ChunkedGrid.GetChunkCellsCenter(clusterBeginId)[beginIndexInChunk];
        }

        private void Debug_PathsInGame()
        {
            if (!ShowPaths) return;
            if (Waypoints.Count == 0) return;
            Gizmos.color = Color.black;
            float size = 0.4f;
            foreach ((_, List<Vector3> points) in Waypoints)
            {
                foreach (Vector3 point in points)
                {
                    Gizmos.DrawSphere(point, size);
                }
            }
        }

        private void ShowStartEndGates()
        {
            if (!ShowStartEndGate) return;
            if (startGatesByDistance.Count == 0 || endGatesByDistance.Count == 0) return;
            
            Gizmos.color = Color.cyan;
            Vector3 size = Vector3.one + Vector3.up;

            foreach ((int i, Vector3 pos) in StartRegiment)
            {
                int startgridIndex = startGatesByDistance[i][0].CellGridIndex;
            
                Vector3 startPosition = startgridIndex.GetCellCenterFromIndex(hpaGrid.MapSize, 1);
                Gizmos.DrawCube(startPosition, size);
                
                int endgridIndex = endGatesByDistance[i][0].CellGridIndex;

                Vector3 endPosition = endgridIndex.GetCellCenterFromIndex(hpaGrid.MapSize, 1);
                Gizmos.DrawCube(endPosition, size);
            }
            /*
            int startgridIndex = startGatesByDistance[0].CellGridIndex;
            
            Vector3 startPosition = startgridIndex.GetCellCenterFromIndex(hpaGrid.MapSize, 1);
            Gizmos.DrawCube(startPosition, size);

            int endgridIndex = endGatesByDistance[0].CellGridIndex;

            Vector3 endPosition = endgridIndex.GetCellCenterFromIndex(hpaGrid.MapSize, 1);
            Gizmos.DrawCube(endPosition, size);
            */
        }
        
    }
}
#endif