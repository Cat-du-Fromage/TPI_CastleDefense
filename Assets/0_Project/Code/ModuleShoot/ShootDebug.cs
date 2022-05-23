using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR


namespace KaizerWald
{
    
    public partial class ShootManager : MonoBehaviour
    {
        public bool DebugShootDirection;
        private void OnDrawGizmos()
        {
            if (!DebugShootDirection) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                Vector3 origin = regiment.UnitsTransform[i].position + Vector3.up;
                Gizmos.DrawSphere(origin, 0.2f);
                
                
                Vector3 direction = regiment.UnitsTransform[i].forward;
                Gizmos.DrawRay(origin, direction*10f);
            }
        }
    }
}
#endif