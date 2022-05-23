using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using static KWUtils.KWmath;

namespace KaizerWald
{
    public class BulletComponent : MonoBehaviour
    {

        [SerializeField] private float muzzleVelocity = 10f;
        private float velocity = 5f;

        public bool Hit;
        
        //Stored Values
        public Vector3 StartPosition;
        private Rigidbody bulletRigidBody;
        
        private TrailRenderer trail;

        public Unit enemyHit;
        private LayerMask hitMask;
        private void Awake()
        {
            StartPosition = transform.position;
            bulletRigidBody = GetComponent<Rigidbody>();
            trail = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            if (bulletRigidBody.velocity == Vector3.zero)
            {
                Destroy(gameObject);
                return;
            }
                
            CheckFadeDistance();
        }

        public void CheckFadeDistance()
        {
            if ((transform.position - StartPosition).sqrMagnitude > 1024f)
            {
                Destroy(gameObject);
            }
        }


        //==============================================================================================================
        //EXTERNAL CALL
        //==============================================================================================================

        public void Shoot(Vector3 start, Vector3 direction, LayerMask mask)
        {
            hitMask = mask;
            StartPosition = start;
            trail.emitting = true;

            bulletRigidBody.velocity = direction * velocity;

            bulletRigidBody.useGravity = true;
            
            bulletRigidBody.AddForce(bulletRigidBody.velocity * muzzleVelocity, ForceMode.Impulse);
        }
        

        private void OnCollisionEnter(Collision other)
        {
            Hit = other.gameObject.layer == math.floorlog2( hitMask.value);
            //Debug.Log($"hitted {other.gameObject.layer}; value {math.floorlog2( hitMask.value)}");
            if (Hit)
            {
                
                other.transform.GetComponent<Unit>().OnDeath();
                Destroy(gameObject);
            }
        }
    }
}
