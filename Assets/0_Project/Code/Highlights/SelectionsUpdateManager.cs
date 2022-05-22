using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
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
            if (coordinator.SelectedRegiments.Count == 0) return;
            if (coordinator.MovingRegiments.Count == 0) return;
            
            selectJobHandles = new NativeList<JobHandle>(coordinator.MovingRegiments.Count, Allocator.Temp);
            
            int realIndex = 0;
            for (int i = 0; i < coordinator.MovingRegiments.Count; i++)
            {
                bool isSelect = coordinator.MovingRegiments[i].IsSelected;
                if (!isSelect) continue;
                TransformAccessArray unitAccessArray = coordinator.MovingRegiments[i].UnitsTransformAccessArray;
                
                JobHandle arrayJobHandle = GetPositionAndRotation(unitAccessArray, out NativeArray<Vector3> positions, out NativeArray<Quaternion> rotations);
                MoveSelections(i, realIndex, rotations, positions, arrayJobHandle);

                rotations.Dispose(selectJobHandles[realIndex]);
                positions.Dispose(selectJobHandles[realIndex]);
                realIndex++;
            }
            
            JobHandle.ScheduleBatchedJobs();
        }
        
        private JobHandle GetPositionAndRotation(TransformAccessArray unitAccessArray, out NativeArray<Vector3> positions, out NativeArray<Quaternion> rotations)
        {
            positions = AllocNtvAry<Vector3>(unitAccessArray.length);
            rotations = AllocNtvAry<Quaternion>(unitAccessArray.length);
            
            //Get Position And Rotation
            JGetPositionAndRotation job = new JGetPositionAndRotation(positions, rotations);
            return job.ScheduleReadOnly(unitAccessArray, JobsUtility.JobWorkerCount - 1);
        }

        private void MoveSelections(int index, int realIndex, in NativeArray<Quaternion> rot, in NativeArray<Vector3> pos, JobHandle arraysD)
        {
            JHighlightsMove selectJob = new JHighlightsMove(pos, rot);
                
            JobHandle dependency = realIndex == 0 ? default : selectJobHandles[realIndex - 1];
            JobHandle combineDependency = JobHandle.CombineDependencies(dependency, arraysD);
            
            TransformAccessArray selectAccessArray =
                ((SelectionRegister)coordinator.SelectionSystem.Register).TransformAccessArrays[coordinator.MovingRegiments[index].RegimentID];
            
            selectJobHandles.Add(selectJob.Schedule(selectAccessArray, combineDependency));
        }
    }
}
