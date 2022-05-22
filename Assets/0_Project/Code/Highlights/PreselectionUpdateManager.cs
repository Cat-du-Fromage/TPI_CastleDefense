using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
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
            if(coordinator.PreselectedRegiments.Count == 0) return;
            if (coordinator.MovingRegiments.Count == 0) return;
            
            preselectJobHandles = new NativeList<JobHandle>(coordinator.MovingRegiments.Count, Allocator.Temp);

            int realIndex = 0;
            for (int i = 0; i < coordinator.MovingRegiments.Count; i++)
            {
                bool isPreselect = coordinator.MovingRegiments[i].IsPreselected;
                if (!isPreselect) continue;
                TransformAccessArray unitAccessArray = coordinator.MovingRegiments[i].UnitsTransformAccessArray;

                NativeArray<Quaternion> rotations = AllocNtvAry<Quaternion>(unitAccessArray.length);
                NativeArray<Vector3> positions = AllocNtvAry<Vector3>(unitAccessArray.length);
                
                for (int j = 0; j < unitAccessArray.length; j++)
                {
                    rotations[j] = unitAccessArray[j].rotation;
                    positions[j] = unitAccessArray[j].position;
                }

                MovePreselections(i,realIndex, rotations, positions);

                
                rotations.Dispose(preselectJobHandles[realIndex]);
                positions.Dispose(preselectJobHandles[realIndex]);
                realIndex += 1;
            }
            JobHandle.ScheduleBatchedJobs();
        }

        private void MovePreselections(int index, int realIndex, in NativeArray<Quaternion> rot, in NativeArray<Vector3> pos)
        {
            JHighlightsMove preselectJob = new JHighlightsMove
            {
                Rotations = rot,
                Positions = pos,
            };
            
            JobHandle dependency;
            if (realIndex == 0)
                dependency = default;
            else
                dependency = preselectJobHandles[realIndex - 1];

            TransformAccessArray preselectAccessArray =
                ((PreselectionRegister)coordinator.PreselectionSystem.Register).TransformAccessArrays[coordinator.MovingRegiments[index].RegimentID];
            
            preselectJobHandles.Add(preselectJob.Schedule(preselectAccessArray, dependency));
        }
        
    }
}
