using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using UnityEngine;

#if UNITY_EDITOR
namespace Kaizerwald
{
    public partial class HPAPathfinder
    {
        public bool ShowEndStart;
        public bool ShowStartEndGate;

        private Vector3 startCellPosition = Vector3.zero;
        private Vector3 endCellPosition = Vector3.zero;
        
        private void OnDrawGizmos()
        {
            InitializeValues();
            DrawStartEnd();
            ShowStartEndGates();
        }

        private void DrawStartEnd()
        {
            if (!ShowEndStart) return;
            InitializeValues();
            float size = 0.4f;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startCellPosition, size);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endCellPosition, size);
        }

        private void InitializeValues()
        {
            if (hpaGrid == null) return;
            if (startCellPosition == Vector3.zero)
            {
                if (StartObject != null)
                {
                    int sGridIndex = StartObject.position.GetIndexFromPosition(hpaGrid.MapSize, 1);
                    int sIndexInChunk = hpaGrid.ChunkedGrid.CellChunkIndexFromGridIndex(sGridIndex);
                    startCellPosition = hpaGrid.ChunkedGrid.GetChunkCellsCenter(clusterStartIndex)[sIndexInChunk];
                }
            }
            
            if (endCellPosition == Vector3.zero)
            {
                if (EndObject != null)
                {
                    int eGridIndex = EndObject.position.GetIndexFromPosition(hpaGrid.MapSize, 1);
                    int eIndexInChunk = hpaGrid.ChunkedGrid.CellChunkIndexFromGridIndex(eGridIndex);
                    endCellPosition = hpaGrid.ChunkedGrid.GetChunkCellsCenter(clusterEndIndex)[eIndexInChunk];
                }
            }
        }

        private void ShowStartEndGates()
        {
            if (!ShowStartEndGate) return;
            Gizmos.color = Color.cyan;
            Vector3 size = Vector3.one + Vector3.up;
            
            int startgridIndex = startGatesByDistance[0].CellGridIndex;
            
            Vector3 startPosition = startgridIndex.GetCellCenterFromIndex(hpaGrid.MapSize, 1);
            Gizmos.DrawCube(startPosition, size);
            
            int endgridIndex = endGatesByDistance[0].CellGridIndex;

            Vector3 endPosition = endgridIndex.GetCellCenterFromIndex(hpaGrid.MapSize, 1);
            Gizmos.DrawCube(endPosition, size);
        }
        
    }
}
#endif