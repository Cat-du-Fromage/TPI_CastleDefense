using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KaizerWald
{
    public static class ShootingBehaviour
    {
        private static bool IsTargetInRange(Regiment r)
        {
            Vector3 centerFirstLineRegiment = (r.Units[0].position + r.Units[r.CurrentLineFormation].position) / 2f;
            Vector3 directionUnit = r.Units[0].position.DirectionTo(r.Units[r.CurrentLineFormation].position);

            float lineSize = r.Units[0].position.DistanceTo(r.Units[r.CurrentLineFormation].position);
            //Bounds rectangleRange = new Bounds(Vector3.Cross(directionUnit, Vector3.up), )
                
            NativeArray<RaycastHit> results = new (1, Allocator.Temp);
            NativeArray<BoxcastCommand> commands = new (1, Allocator.Temp);

            Vector3 directionRegiment = Vector3.Cross(directionUnit, Vector3.up);
            
            Vector3 center = centerFirstLineRegiment + directionRegiment * r.regimentShootRange;
            Vector2 halfExtents = new Vector2(lineSize, r.regimentShootRange) * 0.5f;
            Quaternion orientation = Quaternion.LookRotation(directionRegiment,Vector3.up);
            Vector3 direction = Vector3.forward;
            
            //LayerMask layer = r.IsPlayer ? 
            commands[0] = new BoxcastCommand(center, halfExtents, orientation, direction);

            JobHandle handle = BoxcastCommand.ScheduleBatch(commands, results, 1,  default(JobHandle));
            handle.Complete();
            
            RaycastHit batchedHit = results[0];
            if (batchedHit.collider == null)
            {
                
            }
            
            
            results.Dispose();
            commands.Dispose();
            return false;
        }
    }
}
