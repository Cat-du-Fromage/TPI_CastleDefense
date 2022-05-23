using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KaizerWald
{
    public partial class ShootManager : MonoBehaviour
    {
        private ShootSystem shootSystem;
        private GameObject bulletPrefab;
        
        
        
        [field:SerializeField] private Regiment regiment;
        [field:SerializeField] public Regiment TargetRegiment { get; private set; }
        [field:SerializeField] public LayerMask HitMask { get; private set; }
        [field:SerializeField] public bool HasTarget { get; private set; }
        [field:SerializeField] public bool IsFiring { get; private set; }

        public int NumShooter;
        
        private JobHandle targetHandle;
        private NativeArray<RaycastHit> results;
        private NativeArray<SpherecastCommand> commands;
        
        [field:SerializeField] public AudioSource ShootSound{ get; private set; }
        private void Awake()
        {
            regiment = GetComponent<Regiment>();
            shootSystem = FindObjectOfType<ShootSystem>();
            bulletPrefab = shootSystem.bullet;

            ShootSound = gameObject.AddComponent<AudioSource>();
        }

        private void Start()
        {
            HitMask = regiment.IsPlayer ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player");
        }

        private void Update()
        {
            if (regiment.IsMoving)
            {
                if (!IsFiring) return;
                ShootTarget(false);
                HasTarget = false;
            }
            else if (!HasTarget)
            {
                if(IsFiring)
                    ShootTarget(false);
                GetTarget();
            }
            else if(HasTarget)
            {
                if (IsFiring && NumShooter == regiment.CurrentLineFormation) return;
                ShootTarget(true);
            }
        }

        private void LateUpdate()
        {
            if (!results.IsCreated && !commands.IsCreated) return;
            targetHandle.Complete();

            TargetRegiment = IsTargetAcquire();
            
            results.Dispose();
            commands.Dispose();
        }

        public void ShootBullet(Transform unitTransform)
        {
            Vector3 pos = unitTransform.position + Vector3.up + unitTransform.forward;
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
            bullet.GetComponent<BulletComponent>().Shoot(pos,unitTransform.forward, HitMask);

            if (ShootSound.isPlaying) return;
            if(transform.position.DistanceTo(regiment.Units[regiment.Units.Length / 2].transform.position) > 5)
                transform.position = regiment.Units[regiment.Units.Length / 2].transform.position;
            ShootSound.PlayOneShot(shootSystem.Audio);
        }

        private void ShootTarget(bool state)
        {
            //int numTarget = TargetRegiment.Units.Length;

            NumShooter = regiment.CurrentLineFormation;
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                regiment.Units[i].Animation.SetFire(state);
            }
            IsFiring = state;
        }

        private Regiment IsTargetAcquire()
        {
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].collider != null)
                {
                    HasTarget = true;
                    return results[i].transform.GetComponent<Unit>().RegimentAttach;
                }
            }

            HasTarget = false;
            return null;
        }

        private void GetTarget()
        {
            results = new (regiment.CurrentLineFormation, Allocator.TempJob);
            commands = new (regiment.CurrentLineFormation, Allocator.TempJob);
            for (int i = 0; i < regiment.CurrentLineFormation; i++)
            {
                Vector3 origin = regiment.UnitsTransform[i].position + Vector3.up;
                Vector3 direction = regiment.UnitsTransform[i].forward;
                commands[0] = new SpherecastCommand(origin, 10f,direction, 20f,HitMask);
            }
            targetHandle = SpherecastCommand.ScheduleBatch(commands, results, regiment.CurrentLineFormation, default(JobHandle));
        }

        
    }
}
