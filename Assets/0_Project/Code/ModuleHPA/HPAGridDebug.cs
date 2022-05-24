using System.Collections;
using System.Collections.Generic;
using KWUtils;
using UnityEditor;
using UnityEngine;

//THIS WILL NOT BE COMPILE: DEBUG ONLY
#if UNITY_EDITOR
namespace Kaizerwald
{
    public partial class HPAGrid
    {
        //GizmosDebug
        [SerializeField] private bool ChunkGridDebug;
        [SerializeField] private bool TestGateEntry = false;
        // Start is called before the first frame update
        private void OnDrawGizmos()
        {
            ChunkGridGizmos();
            GizmosGateEntry();
            GizmosGatesPath();
        }

        private void ChunkGridGizmos()
        {
            if (!ChunkGridDebug || ChunkedGrid == null) return;
            Gizmos.color = Color.red;
            Vector3 size = new Vector3(ChunkSize, 1, ChunkSize);
            for (int i = 0; i < ChunkedGrid.ChunkDictionary.Count; i++)
            {
                Gizmos.DrawWireCube(ChunkedGrid.GetChunkCenter(i), size);
            }
        }

        private void GizmosGateEntry()
        {
            if (!TestGateEntry) return;
            Gizmos.color = Color.green;
            for (int i = 0; i < Clusters.Count; i++)
            {
                for (int j = 0; j < Clusters[i].Gates.Length; j++)
                {
                    int chunkIndex = Clusters[i].ChunkIndex;
                    WaypointCardinal cardinal = (WaypointCardinal)j;
                    
                    for (int k = 0; k < Clusters[i].GetNumGatesAtCardinal(cardinal); k++)
                    {
                        Gate gate = Clusters[i].GetGateAt(cardinal, k);
                        
                        Vector3 center = ChunkedGrid.GetChunkCellCenter(chunkIndex, gate.IndexInChunk);
                        
                        Gizmos.DrawCube(center, Vector3.one);
                    }
                }
            }
        }

        private void GizmosGatesPath()
        {
            if (!TestGateEntry) return;

            Vector3 offset = Vector3.right * 0.1f;
            
            Gizmos.color = Color.magenta;
            //Cluster
            for (int i = 0; i < Clusters.Count; i++)
            {
                //Gates
                for (int j = 0; j < Clusters[i].AllGates.Count; j++)
                {
                    //Paths
                    for (int k = 0; k < Clusters[i].AllGates[j].PathsToGates.Count; k++)
                    {
                        //NodePaths
                        for (int l = 0; l < Clusters[i].AllGates[j].PathsToGates[k].NodesPath.Length; l++)
                        {
                            Vector3 center = ChunkedGrid.GetChunkCellCenter(i, Clusters[i].AllGates[j].PathsToGates[k].NodesPath[l]);
                            Gizmos.DrawWireSphere(center, 0.2f);
                            //center += offset * k + Vector3.up;
                            //Handles.Label(center, $"{k}");
                        }
                    }
                }
            }
        }
    }
}
#endif