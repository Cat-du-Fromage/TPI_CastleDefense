using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

using static KWUtils.NativeCollectionExt;

namespace KaizerWald
{
    public class RegimentUpdateManager : MonoBehaviour
    {
        private IHighlightCoordinator coordinator;
        private RegimentManager regimentManager;

        private NativeArray<JobHandle> jobHandles;
        
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
            if (coordinator.MovingRegiments.Count == 0) return;
            
            jobHandles = AllocNtvAry<JobHandle>(coordinator.MovingRegiments.Count, Allocator.Temp);

            for (int i = 0; i < coordinator.MovingRegiments.Count; i++)
            {
                TransformAccessArray accessArray = coordinator.PlacementAccessArrays[coordinator.MovingRegiments[i].RegimentID];

                NativeArray<Quaternion> rotations = AllocNtvAry<Quaternion>(accessArray.length);
                NativeArray<Vector3> positions = AllocNtvAry<Vector3>(accessArray.length);
                
                for (int j = 0; j < accessArray.length; j++)
                {
                    rotations[j] = accessArray[j].rotation;
                    positions[j] = accessArray[j].position;
                }
                
                JMoveEnemies job = new JMoveEnemies
                {
                    Speed = 1,
                    DeltaTime = Time.deltaTime,
                    GoalsRotation = rotations,
                    GoalsPosition = positions,
                };
                
                JobHandle dependency = i == 0 ? default : jobHandles[i - 1];
                jobHandles[i] = job.Schedule(coordinator.MovingRegiments[i].UnitsTransformAccessArray, dependency);
                
                rotations.Dispose(jobHandles[i]);
                positions.Dispose(jobHandles[i]);
            }
            
            JobHandle.ScheduleBatchedJobs();
        }

        private void LateUpdate()
        {
            if (coordinator.MovingRegiments.Count == 0) return;
            CheckReachGoal();
        }

        private void CheckReachGoal()
        {
            for (int i = coordinator.MovingRegiments.Count - 1; i > -1; i--)
            {
                TransformAccessArray goalAccessArray = coordinator.PlacementAccessArrays[coordinator.MovingRegiments[i].RegimentID];
                TransformAccessArray unitAccessArray = coordinator.MovingRegiments[i].UnitsTransformAccessArray;

                int numUnitsReach = 0;
                for (int j = 0; j < coordinator.MovingRegiments[i].Units.Length; j++)
                {
                    float distance = goalAccessArray[j].position.Flat().DistanceTo(unitAccessArray[j].position.Flat());
                    if(distance > 0.1f) continue;
                    numUnitsReach += 1;
                }
                
                if (numUnitsReach != coordinator.MovingRegiments[i].Units.Length) continue;
                coordinator.MovingRegiments[i].SetMoving(false);
                coordinator.MovingRegiments.Remove(coordinator.MovingRegiments[i]);
            }
        }
    }
    
    [BurstCompile(CompileSynchronously = true)]
    public struct JMoveEnemies : IJobParallelForTransform
    {
        [ReadOnly] public float Speed;
        [ReadOnly] public float DeltaTime;

        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<Quaternion> GoalsRotation;
        
        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<Vector3> GoalsPosition;

        public void Execute(int index, TransformAccess transform)
        {
            //Rotation
            
            quaternion rotation = math.slerp(transform.rotation, GoalsRotation[index], DeltaTime * Speed);
            transform.rotation = rotation;//Quaternion.Slerp(transform.rotation, GoalsRotation[index], DeltaTime * Speed);
            
            //Position
            //Vector3 newPosition = Vector3.Lerp(transform.position, GoalsPosition[index], (DeltaTime * Speed));
            float2 goalPos = new float2(GoalsPosition[index].x, GoalsPosition[index].z);
            float2 currentPos = new float2(transform.position.x, transform.position.z);
            float2 direction =  math.normalizesafe(goalPos - currentPos);
            
            Vector3 newPosition = transform.position + (DeltaTime * Speed) * new Vector3(direction.x, 0.0f, direction.y);
            //Apply Changes to GameObject's Transform
            transform.position = newPosition;
        }
    }
}
