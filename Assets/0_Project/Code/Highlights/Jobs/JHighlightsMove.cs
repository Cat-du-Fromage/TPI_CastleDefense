using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    [BurstCompile(CompileSynchronously = true)]
    public struct JHighlightsMove : IJobParallelForTransform
    {
        [NativeDisableParallelForRestriction]
        [ReadOnly] private NativeArray<Vector3> positions;
        
        [NativeDisableParallelForRestriction]
        [ReadOnly] private NativeArray<Quaternion> rotations;
        
        public JHighlightsMove(NativeArray<Vector3> pos, NativeArray<Quaternion> rot)
        {
            positions = pos;
            rotations = rot;
        }
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.rotation = rotations[index];
            transform.position = new Vector3(positions[index].x, transform.position.y, positions[index].z);
        }
    }
}