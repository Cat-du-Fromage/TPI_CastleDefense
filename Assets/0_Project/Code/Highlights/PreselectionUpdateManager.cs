using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using static KWUtils.NativeCollectionExt;

namespace KaizerWald
{
    [RequireComponent(typeof(RegimentManager))]
    public class PreselectionUpdateManager : MonoBehaviour
    {
        private IHighlightCoordinator coordinator;
        private RegimentManager regimentManager;

        private NativeList<JobHandle> preselectJobHandles;

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
            if (coordinator.PreselectedRegiments.Count == 0) return;
            if (coordinator.MovingRegiments.Count == 0) return;
            
            preselectJobHandles = new NativeList<JobHandle>(coordinator.MovingRegiments.Count, Allocator.Temp);

            int realIndex = 0;
            for (int i = 0; i < coordinator.MovingRegiments.Count; i++)
            {
                bool isPreselect = coordinator.MovingRegiments[i].IsPreselected;
                if (!isPreselect) continue;
                TransformAccessArray unitAccessArray = coordinator.MovingRegiments[i].UnitsTransformAccessArray;
                
                JobHandle arrayJobHandle = GetPositionAndRotation(unitAccessArray, out NativeArray<Vector3> positions, out NativeArray<Quaternion> rotations);
                MovePreselections(i, realIndex, rotations, positions, arrayJobHandle);

                rotations.Dispose(preselectJobHandles[realIndex]);
                positions.Dispose(preselectJobHandles[realIndex]);
                realIndex++;
            }
            JobHandle.ScheduleBatchedJobs();
        }
        private JobHandle GetPositionAndRotation(TransformAccessArray unitAccessArray, out NativeArray<Vector3> positions ,out NativeArray<Quaternion> rotations)
        {
            positions = AllocNtvAry<Vector3>(unitAccessArray.length);
            rotations = AllocNtvAry<Quaternion>(unitAccessArray.length);

            //Get Position And Rotation
            JGetPositionAndRotation job = new JGetPositionAndRotation(positions, rotations);
            return job.ScheduleReadOnly(unitAccessArray, JobsUtility.JobWorkerCount - 1);
        }

        private void MovePreselections(int index, int realIndex, in NativeArray<Quaternion> rot, in NativeArray<Vector3> pos, JobHandle arraysD)
        {
            JHighlightsMove preselectJob = new JHighlightsMove(pos, rot);

            JobHandle dependency = realIndex == 0 ? default : preselectJobHandles[realIndex - 1];
            JobHandle combineDependency = JobHandle.CombineDependencies(dependency, arraysD);
            
            TransformAccessArray preselectAccessArray =
                ((PreselectionRegister)coordinator.PreselectionSystem.Register).TransformAccessArrays[coordinator.MovingRegiments[index].RegimentID];
            
            preselectJobHandles.Add(preselectJob.Schedule(preselectAccessArray, combineDependency));
        }
        
    }
}
