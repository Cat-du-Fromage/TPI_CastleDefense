using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using static KWUtils.NativeCollectionExt;

namespace KaizerWald
{
    [RequireComponent(typeof(RegimentManager))]
    public class SelectionsUpdateManager : MonoBehaviour
    {
        private IHighlightCoordinator coordinator;
        private RegimentManager regimentManager;

        //private NativeList<JobHandle> preselectJobHandles;
        private NativeList<JobHandle> selectJobHandles;
        
        private void Awake()
        {
            regimentManager = GetComponent<RegimentManager>();
        }
        
        private void Start()
        {
            IHighlightCoordinator coord = (IHighlightCoordinator)regimentManager;
            coordinator = coord;
        }

        private void Update()
        {
            if(coordinator.SelectedRegiments.Count == 0) return;
            if (coordinator.MovingRegiments.Count == 0) return;
            
            selectJobHandles = new NativeList<JobHandle>(coordinator.MovingRegiments.Count, Allocator.Temp);
            
            int realIndex = 0;
            for (int i = 0; i < coordinator.MovingRegiments.Count; i++)
            {
                bool isSelect = coordinator.MovingRegiments[i].IsSelected;
                if (!isSelect) continue;
                
                TransformAccessArray unitAccessArray = coordinator.MovingRegiments[i].UnitsTransformAccessArray;

                NativeArray<Quaternion> rotations = AllocNtvAry<Quaternion>(unitAccessArray.length);
                NativeArray<Vector3> positions = AllocNtvAry<Vector3>(unitAccessArray.length);
                
                for (int j = 0; j < unitAccessArray.length; j++)
                {
                    rotations[j] = unitAccessArray[j].rotation;
                    positions[j] = unitAccessArray[j].position;
                }

                MoveSelections(i, realIndex, rotations, positions);

                rotations.Dispose(selectJobHandles[realIndex]);
                positions.Dispose(selectJobHandles[realIndex]);
                realIndex += 1;
            }
            
            JobHandle.ScheduleBatchedJobs();
        }
        
        private void MoveSelections(int index, int realIndex, in NativeArray<Quaternion> rot, in NativeArray<Vector3> pos)
        {
            JHighlightsMove selectJob = new JHighlightsMove
            {
                Rotations = rot,
                Positions = pos,
            };
            
            JobHandle dependency;
            if (realIndex == 0 || selectJobHandles.Length == 0)
                dependency = default;
            else
                dependency = selectJobHandles[realIndex - 1];
            
            TransformAccessArray selectAccessArray =
                ((SelectionRegister)coordinator.SelectionSystem.Register).TransformAccessArrays[coordinator.MovingRegiments[index].RegimentID];
            
            selectJobHandles.Add(selectJob.Schedule(selectAccessArray, dependency));
        }

    }

    public struct JHighlightsMove : IJobParallelForTransform
    {
        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<Quaternion> Rotations;
        
        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<Vector3> Positions;
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.rotation = Rotations[index];
            transform.position = new Vector3(Positions[index].x, transform.position.y, Positions[index].z);
        }
    }
}
