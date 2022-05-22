using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    [BurstCompile(CompileSynchronously = true)]
    public struct JGetPositionAndRotation : IJobParallelForTransform
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly]private NativeArray<Vector3> positions;
        [NativeDisableParallelForRestriction]
        [WriteOnly]private NativeArray<Quaternion> rotations;
        
        public JGetPositionAndRotation(NativeArray<Vector3> pos, NativeArray<Quaternion> rot)
        {
            positions = pos;
            rotations = rot;
        }
        
        public void Execute(int index, TransformAccess transform)
        {
            positions[index] = transform.position;
            rotations[index] = transform.rotation;
        }
    }
}