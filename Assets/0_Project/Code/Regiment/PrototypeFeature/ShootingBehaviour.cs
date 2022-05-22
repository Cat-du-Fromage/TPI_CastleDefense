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
        public static bool IsTargetInRange(Regiment r, out RaycastHit hit)
        {
            Vector3 centerFirstLineRegiment = (r.UnitsTransform[0].position + r.UnitsTransform[r.CurrentLineFormation].position) / 2f;
            Vector3 directionUnit = r.UnitsTransform[0].position.DirectionTo(r.UnitsTransform[r.CurrentLineFormation].position).normalized;

            float lineSize = r.UnitsTransform[0].position.DistanceTo(r.UnitsTransform[r.CurrentLineFormation].position);
            //Bounds rectangleRange = new Bounds(Vector3.Cross(directionUnit, Vector3.up), )
                
            NativeArray<RaycastHit> results = new (1, Allocator.TempJob);
            NativeArray<BoxcastCommand> commands = new (1, Allocator.TempJob);

            Vector3 directionRegiment = Vector3.Cross(directionUnit, Vector3.up).normalized;
            
            Vector3 center = centerFirstLineRegiment + directionRegiment * r.regimentShootRange;
            Vector2 halfExtents = new Vector2(lineSize, r.regimentShootRange) * 0.5f;
            Quaternion orientation = Quaternion.LookRotation(directionRegiment,Vector3.up);
            Vector3 direction = directionRegiment;

            LayerMask layer = r.IsPlayer ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");
            if (r.IsPlayer)
            {
                layer = LayerMask.NameToLayer("Enemy");
            }
            else
            {
                layer =LayerMask.NameToLayer("Player");
            }
            
            commands[0] = new BoxcastCommand(center, halfExtents, orientation, direction, layer);

            JobHandle handle = BoxcastCommand.ScheduleBatch(commands, results, 1,  default(JobHandle));
            handle.Complete();
            
            RaycastHit batchedHit = results[0];
            hit = batchedHit;
            results.Dispose();
            commands.Dispose();
            if (batchedHit.collider == null)
            {
                return false;
            }
            return true;
        }
    }
}
